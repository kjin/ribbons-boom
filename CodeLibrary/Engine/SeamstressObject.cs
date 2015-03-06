using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using CodeLibrary.Audio;
using CodeLibrary.Graphics;

namespace CodeLibrary.Engine
{
    public class SeamstressObject : GameObject, IAudible
    {

        #region Constants
        // Sensor Constants
        public const String SENSOR_NAME = "SeamstressGroundSensor";
        private const float SENSOR_HEIGHT = 0.1f;
        private const float SENSOR_WIDTH_COEF = 0.447f;

        // vvvvvv Physics, Movement, and Cooldown constants can go into ConstantsController
        // Physics constants
        public float DEFAULT_DENSITY = 1f;// 'const' removed for ability to change

        // Movement constants
        public float SEAMSTRESS_GROUNDSPEED = PhysicsConstants.SEAM_GROUNDSPEED;    // 'const' removed for ability to change
        public float SEAMSTRESS_AIRSPEED = PhysicsConstants.SEAM_AIRSPEED;
        //private float SEAMSTRESS_MAXSPEED = 6.0f;  // 'const' removed for ability to change
        public static float PRIMARY_JUMPFORCE = PhysicsConstants.SEAM_PRIMARY_JUMPFORCE;// 'const' removed for ability to change
        public static float SECONDARY_JUMPFORCE = PhysicsConstants.SEAM_SECONDARY_JUMPFORCE;// 'const' removed for ability to change

        private float SEAMSTRESS_AERIALDAMPING = PhysicsConstants.SEAMSTRESS_HORIZONTAL_AIRDRAG; // 'const' removed for ability to change
        public bool APPLY_AERIALDAMPING_VERTICALLY = true;
        private float SEAMSTRESS_GROUNDDAMPING = 0.2f;// 'const' removed for ability to change

        // Cooldown constants
        public int MAX_JUMP_COOLDOWN = 8; // 'const' removed for ability to change
        // ^^^^^^ Physics, Movement, and Cooldown constants can go into ConstantsController

        protected const int MAX_FRAME = 4;
        protected const int FRAME_SPEED = 10;
        protected const int IDLE_FRAME_LIMIT = 20;

        private Vector2 boundingBoxDimensions = new Vector2(0.5f, 1);

        public int spawnID;
        #endregion

        #region Properties

        /// <summary>
        /// Number of frames seamstress has been grounded for picking out standing animation in level editor.
        /// </summary>
        public int FramesGrounded
        {
            set { framesGrounded = value; }
        }

        public int RibbonActiveTime { get { return ribbonActiveTime; } set { ribbonActiveTime = value; } }

        public RibbonObject Ribbon
        {
            get { return ribbon; }
            set { if (value != null && !value.Equals(ribbon)) ribbonActiveTime = 0; ribbon = value; }
        }

        /// <summary>
        /// Name of the ground sensor (used for Farseer)
        /// </summary>
        public String SensorName
        {
            get { return SENSOR_NAME; }
        }

        /// <summary>
        /// Left/right movement of this character.  Result of input times seamstress force.
        /// </summary>
        public float Movement
        {
            get { return movement; }
            set
            {
                movement = value;
                // Change facing if appropriate
                if (movement < 0)
                {
                    facingLeft = false;
                }
                else if (movement > 0)
                {
                    facingLeft = true;
                }
            }
        }

        public int ID { get { return id; } set { id = value; } }

        /// <summary>
        /// Whether the seamstress is actively jumping.
        /// </summary>
        public bool IsJumping
        {
            get { return isJumping && jumpCooldown <= 0 && IsGrounded; }
            set { isJumping = value; }
        }

        /// <summary>
        /// Whether or not this dude is touching the ground
        /// </summary>
        public bool IsGrounded
        {
            get { return framesGrounded >= 0; }
            set
            {
                if (value && framesGrounded == -1)
                    framesGrounded = 0;
                else if (!value)
                    framesGrounded = -1;
            }
        }

        /// <summary>
        /// Whether or not the seamstress is walking
        /// </summary>
        public bool IsWalking
        {
            get { return isWalking; }
            set { isWalking = value; }
        }


        /// <summary>
        /// How hard the brakes are applied to get a SEAMSTRESS to stop moving
        /// </summary>
        public float AerialDamping
        {
            get { return SEAMSTRESS_AERIALDAMPING; }
            set { SEAMSTRESS_AERIALDAMPING = value; }
        }

        /// <summary>
        /// How hard the brakes are applied to get a SEAMSTRESS to stop moving
        /// </summary>
        public float GroundDamping
        {
            get { return SEAMSTRESS_GROUNDDAMPING; }
            set { SEAMSTRESS_GROUNDDAMPING = value; }
        }

        /// <summry>
        /// Added methods to set other constants for technical prototype
        /// </summry>
        public int Cooldown
        {
            set { MAX_JUMP_COOLDOWN = value; }
        }

        public float Density
        {
            set { DEFAULT_DENSITY = value; }
        }

        /// <summary>
        /// Shape of the ground detecting sensor.
        /// </summary>
        public Fixture SensorFixture
        {
            get { return sensorFixture; }
        }

        /// <summary>
        /// Whether this character is facing left
        /// </summary>
        public bool FacingLeft
        {
            get { return facingLeft; }
            set { facingLeft = value;  }
        }

        /// <summary>Speed of ribbon movement with respect to whether
        /// Seamstress isRibboned. Used to calculate forces on Seamstress from
        /// contact with the Ribbon.</summary>
        public Vector2 RibbonSpeed
        {
            set { ribbonSpeed = value; }
            get { return ribbonSpeed; }
        }

        public int FramesIdle
        {
            get { return framesIdle; }
            set { framesIdle = value; }
        }

        public SeamstressSprites SeamstressSprites
        {
            get { return seamstressSprites; }
        }
        #endregion

        #region Fields
        // The physics body
        public Body body;

        // Stuff from BoxObject 
        public Vector2 dimension; // size of the sprite (and the physics shape)
        public Vector2 scale;
        public Fixture fixture;

        // For ribbon movement
        public bool isRibboned;
        private Vector2 ribbonSpeed; // speed of ribbon for calculating ribbon force
        public Vector2 prevRibbonSpeed;
        public float ribbonFloatSpeed;

        protected int frame;
        protected int frameSwitch;
        protected int framesIdle;
        protected bool isIdle;

        // Cooldown for seamstress abilities
        private float movement;
        private bool facingLeft;
        private bool isWalking;

        public int jumpCooldown;
        private bool isJumping;

        // Ground sensor to represent feet
        private Fixture sensorFixture;
        private int framesGrounded;

        // debugging
        private List<Vector2> hitbox;

        // Seamstress Sprite Object
        SeamstressSprites seamstressSprites;
        SeamstressSounds seamstressSounds;
        private int animationFrames;

        public bool alive;
        public bool win;
        DeathMethods deathMethod;

        private RibbonObject ribbon;
        private int ribbonActiveTime;
        int id;

        #endregion

        #region Methods

        /// <summary>
        /// Create a new seamstress object. Activate Physics.
        /// </summary>
        public SeamstressObject(SeamstressSprites seamstressSprites, SeamstressSounds seamstressSounds, World w)
        {
            this.seamstressSprites = seamstressSprites;
            this.seamstressSprites.Land.TicksPerFrame = 3;
            this.seamstressSounds = seamstressSounds;
            animationFrames = 4;
            ribbonFloatSpeed = 0f;
            
            this.dimension = new Vector2((float)seamstressSprites.Stand.Width, (float)seamstressSprites.Stand.Height);
            this.scale = new Vector2(dimension.X / seamstressSprites.Stand.Width, dimension.Y / seamstressSprites.Stand.Height);

            body = BodyFactory.CreateBody(w, this);
            // initialize physics stuff if body was successfully created
            if (body != null)
            {
                body.BodyType = BodyType.Static;
                body.Position = new Vector2(0,0);
                body.Rotation = 0;
                body.UserData = this;
                CreateShape(1.0f);
            }
            
            // Physics attributes
            body.BodyType = BodyType.Dynamic;
            //CreateShape(DEFAULT_DENSITY);

            // Gameplay attributes
            framesGrounded = -1;
            isRibboned = false;
            isWalking = false;
            isJumping = false;

            jumpCooldown = 0;
            frame = 0;
            frameSwitch = 0;
            framesIdle = 0;

            //Initialize Physics
            // create the box from our superclass
            body.FixedRotation = true;

            // Ground Sensor
            Vector2 sensorCenter = new Vector2(0, dimension.Y / 2);
            sensorFixture = FixtureFactory.AttachRectangle(dimension.X * SENSOR_WIDTH_COEF, SENSOR_HEIGHT, DEFAULT_DENSITY, sensorCenter, body, SensorName);
            sensorFixture.IsSensor = true;
            sensorFixture.CollisionCategories = PhysicsConstants.SEAMSTRESS_CATEGORY;

            alive = true;

            ribbon = null;

            //hitbox = new List<Vector2>();
        }

        public void Update(float dt)
        {
            if (!IsGrounded)
            {
                if (body.LinearVelocity.X > 0.05f)
                    facingLeft = false;
                if (body.LinearVelocity.X < -0.05f)
                    facingLeft = true;
            }
            else
                framesGrounded++;
            if (body.LinearVelocity.X == 0)
                animationFrames = GraphicsConstants.DEFAULT_ANIMATION_SPEED;
            else
                animationFrames = Math.Max((int)(seamstressSprites.Run.Frames / (float)Math.Round(Math.Abs(body.LinearVelocity.X))), 1);
            
            //idle counter
            framesIdle++;
            if (framesIdle > IDLE_FRAME_LIMIT) { isIdle = true; } else { isIdle = false; }
            if (framesIdle == int.MaxValue) { framesIdle = IDLE_FRAME_LIMIT + 1; }

            if (ribbon != null)
                ribbonActiveTime++;

            RibbonSpeed = new Vector2(0, 0);
        }

        public void Draw(Canvas c)
        {
            if (alive)
            {
                if (IsGrounded)
                {
                    if (framesGrounded <= 2)
                    {
                        seamstressSprites.Land.Reset();
                    }
                    if (seamstressSprites.Land.AnimationProgress < 0.8f)
                    {
                        c.DrawSprite(seamstressSprites.Land, Color.White, body.Position, 0, 1, facingLeft);
                    }
                    else
                    {
                        if (isWalking)
                        {
                            seamstressSprites.Run.TicksPerFrame = 3;
                            c.DrawSprite(seamstressSprites.Run, Color.White, body.Position, 0, 1, facingLeft);
                        }
                        else if (isIdle)
                        {
                            seamstressSprites.Idle.TicksPerFrame = 10;
                            c.DrawSprite(seamstressSprites.Idle, Color.White, body.Position, 0, 1, facingLeft);
                        }
                        else
                        {
                            c.DrawSprite(seamstressSprites.Stand, Color.White, body.Position, 0, 1, !facingLeft);
                        }
                    }
                }
                else
                {
                    if (body.LinearVelocity.Y < 0)
                        c.DrawSprite(seamstressSprites.Jump, Color.White, body.Position, 0, 1, facingLeft);
                    else
                        c.DrawSprite(seamstressSprites.Fall, Color.White, body.Position, 0, 1, facingLeft);
                }
            }
            else
            {
                c.DrawSprite(seamstressSprites.Stun, Color.White, body.Position, 0, 1, facingLeft);
            }
            //c.DrawRectangle(Color.Lime, 2, body.Position, boundingBoxDimensions, 0, true);//body.FixtureList[0].Shape

#if DEBUG
            List<Vector2> transformedHitbox = new List<Vector2>();
            foreach (Vector2 v in hitbox)
            {
                transformedHitbox.Add(v + body.Position);
            }
            c.DrawPolygon(Color.LightSteelBlue, 2, transformedHitbox, false);
#endif
            c.CoordinateMode = CoordinateMode.ScreenCoordinates;
            c.CoordinateMode = CoordinateMode.PhysicalCoordinates;
        }

        public void DrawSelection(Canvas c)
        {
            c.DrawRectangle(Color.LightGreen, 4, body.Position, new Vector2(seamstressSprites.Stand.Width, seamstressSprites.Stand.Height), body.Rotation, false);
        }

        public void PlaySound(AudioPlayer audioPlayer)
        {
            //audioPlayer.PlayOnSetTrue(seamstressSounds.Jump, !IsGrounded, 0.5f);
            audioPlayer.PlayOnSetTrue(seamstressSounds.Land, IsGrounded, 0.5f);
            audioPlayer.RepeatOnTrue(seamstressSounds.Step, IsGrounded && isWalking, 3 * seamstressSprites.Run.Frames / 2, 0.1f);
            audioPlayer.PlayOnSetTrue(seamstressSounds.SwapRibbons, ribbonActiveTime > 1, 0.1f);
        }

        /// <summary>
        /// Create the Seamstress's bounding box. This overrides the parent class, because we want to make the
        /// Seamstress's bounding box slightly smaller than her sprite (so it wraps around her more tightly).
        /// </summary>
        protected void CreateShape(float density)
        {
            /*if (fixture != null)
                body.DestroyFixture(fixture);
            Vertices rectangleVertices = PolygonTools.CreateRectangle(boundingBoxDimensions.X / 2, boundingBoxDimensions.Y / 2);
            Vertices bodyVertices = new Vertices();

            float x = PhysicsConstants.SEAM_WIDTH/2 - PhysicsConstants.SEAM_EPSILON;
            float y = 0.5f - PhysicsConstants.SEAM_EPSILON;
            hitbox = new List<Vector2>();

            bodyVertices.Add(new Vector2(-x,-y));
            hitbox.Add(new Vector2(-x, -y));

            bodyVertices.Add(new Vector2(-x + PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));
            hitbox.Add(new Vector2(-x + PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));

            bodyVertices.Add(new Vector2(0, y));
            hitbox.Add(new Vector2(0, y));

            bodyVertices.Add(new Vector2(x - PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));
            hitbox.Add(new Vector2(x - PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));

            /*bodyVertices.Add(new Vector2(-x, y - PhysicsConstants.SEAM_SLANT2));
            hitbox.Add(new Vector2(-x, y - PhysicsConstants.SEAM_SLANT2));

            bodyVertices.Add(new Vector2(-x + PhysicsConstants.SEAM_SLANT2, y));
            hitbox.Add(new Vector2(-x + PhysicsConstants.SEAM_SLANT2, y));

            bodyVertices.Add(new Vector2(x - PhysicsConstants.SEAM_SLANT2, y));
            hitbox.Add(new Vector2(x - PhysicsConstants.SEAM_SLANT2, y));

            bodyVertices.Add(new Vector2(x, y - PhysicsConstants.SEAM_SLANT2));
            hitbox.Add(new Vector2(x, y - PhysicsConstants.SEAM_SLANT2));

            bodyVertices.Add(new Vector2(-x, y));
            hitbox.Add(new Vector2(-x, y));

            bodyVertices.Add(new Vector2(x, y));
            hitbox.Add(new Vector2(x, y));

            bodyVertices.Add(new Vector2(x, -y));
            hitbox.Add(new Vector2(x, -y));
             
            bodyVertices.Add(new Vector2(-dimension.X / 4, -dimension.Y / 2 + PhysicsConstants.SEAM_EPSILON));
            bodyVertices.Add(new Vector2(-dimension.X / 4, dimension.Y / 4 - PhysicsConstants.SEAM_EPSILON / 2));
            bodyVertices.Add(new Vector2(0, dimension.Y / 2 - PhysicsConstants.SEAM_EPSILON));
            bodyVertices.Add(new Vector2(dimension.X / 4, dimension.Y / 4 - PhysicsConstants.SEAM_EPSILON));
            bodyVertices.Add(new Vector2(dimension.X / 4, -dimension.Y / 2 + PhysicsConstants.SEAM_EPSILON));

            //PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
            PolygonShape polygonShape = new PolygonShape(bodyVertices, density);
            fixture = body.CreateFixture(polygonShape, this);
            fixture.CollisionCategories = PhysicsConstants.SEAMSTRESS_CATEGORY;
            fixture.UserData = this;*/

            if (fixture != null)
                body.DestroyFixture(fixture);
            Vertices rectangleVertices = PolygonTools.CreateRectangle(boundingBoxDimensions.X / 2, boundingBoxDimensions.Y / 2);
            Vertices bodyVertices = new Vertices();

            float x = PhysicsConstants.SEAM_WIDTH / 2 - PhysicsConstants.SEAM_EPSILON;
            float y = 0.5f - PhysicsConstants.SEAM_EPSILON;
            hitbox = new List<Vector2>();

            bodyVertices.Add(new Vector2(-x, -y));
            hitbox.Add(new Vector2(-x, -y));

            bodyVertices.Add(new Vector2(-x + PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));
            hitbox.Add(new Vector2(-x + PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));

            bodyVertices.Add(new Vector2(0, y));
            hitbox.Add(new Vector2(0, y));

            bodyVertices.Add(new Vector2(x - PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));
            hitbox.Add(new Vector2(x - PhysicsConstants.SEAM_SLANT, y - PhysicsConstants.SEAM_SLANT));

            bodyVertices.Add(new Vector2(x, -y));
            hitbox.Add(new Vector2(x, -y));

            /*
            bodyVertices.Add(new Vector2(-dimension.X / 4, -dimension.Y / 2 + PhysicsConstants.SEAM_EPSILON));
            bodyVertices.Add(new Vector2(-dimension.X / 4, dimension.Y / 4 - PhysicsConstants.SEAM_EPSILON / 2));
            bodyVertices.Add(new Vector2(0, dimension.Y / 2 - PhysicsConstants.SEAM_EPSILON));
            bodyVertices.Add(new Vector2(dimension.X / 4, dimension.Y / 4 - PhysicsConstants.SEAM_EPSILON));
            bodyVertices.Add(new Vector2(dimension.X / 4, -dimension.Y / 2 + PhysicsConstants.SEAM_EPSILON));*/

            //PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
            PolygonShape polygonShape = new PolygonShape(bodyVertices, density);
            fixture = body.CreateFixture(polygonShape, this);
            fixture.CollisionCategories = PhysicsConstants.SEAMSTRESS_CATEGORY;
            fixture.UserData = this;
        }

        public void Die(DeathMethods deathMethod)
        {
            if (alive)
            {
                body.LinearVelocity *= new Vector2(1, -1);
                body.IsSensor = true;
            }
            alive = false;
            if (fixture != null)
            {
                body.DestroyFixture(fixture);
                fixture = null;
            }
            if (sensorFixture != null)
            {
                body.DestroyFixture(sensorFixture);
                sensorFixture = null;
            }
            this.deathMethod = deathMethod;
        }

        public void Win()
        {
            win = true;
        }

        public DeathMethods DeathMethod { get { return deathMethod; } set { deathMethod = value; } }

        #endregion

        
    }

    public enum DeathMethods
    {
        Falling, Miasma, Shooter, Spikes
    }
}
