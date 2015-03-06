using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XMLContent;
using System.Xml;
using Microsoft.Xna.Framework;

namespace CodeLibrary.Engine
{
    public class XMLGenerator
    {
        public static void ToXML(Level l, String path)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml("<XnaContent></XnaContent>");
            XmlElement asset = doc.CreateElement("Asset");
            doc.DocumentElement.AppendChild(asset) ;
            asset.SetAttribute("Type", "XMLContent.Level");
            XmlElement level = doc.CreateElement("levelNum");
            XmlElement seamstress = doc.CreateElement("seamstress");
            XmlElement ground = doc.CreateElement("ground");
            XmlElement miasma = doc.CreateElement("miasma");
            XmlElement gameGrid = doc.CreateElement("gameGrid");
            XmlElement ribbons = doc.CreateElement("ribbons");
            XmlElement cameras = doc.CreateElement("cameras");
            XmlElement ribbonBoxes = doc.CreateElement("ribbonBoxes");
            XmlElement shooters = doc.CreateElement("shooters");
            XmlElement needles = doc.CreateElement("needles");
            XmlElement hooks = doc.CreateElement("hooks");
            XmlElement telescopingBlocks = doc.CreateElement("telescopingBlocks");
            XmlElement flipbars = doc.CreateElement("flipbars");
            XmlElement rotatingPlatforms = doc.CreateElement("rotatingPlatforms");
            XmlElement saveRocks = doc.CreateElement("saveRocks");

            level.InnerText = l.levelNum.ToString();

            XmlElement seam_pos = doc.CreateElement("position");
            seam_pos.InnerText = l.seamstress.position.X + " " + l.seamstress.position.Y;
            seamstress.AppendChild(seam_pos);

            foreach(Ground g in l.ground)
            {
                XmlElement g_item = doc.CreateElement("Item");
                XmlElement g_pos = doc.CreateElement("position");
                XmlElement g_dim = doc.CreateElement("dimensions");

                g_pos.InnerText = g.position.X + " " + g.position.Y;
                g_dim.InnerText = g.dimensions.X + " " + g.dimensions.Y;

                ground.AppendChild(g_item);
                g_item.AppendChild(g_pos);
                g_item.AppendChild(g_dim);
            }

            foreach (Miasma m in l.miasma)
            {
                XmlElement m_item = doc.CreateElement("Item");
                XmlElement m_pos = doc.CreateElement("position");
                XmlElement m_dim = doc.CreateElement("dimensions");

                m_pos.InnerText = m.position.X + " " + m.position.Y;
                m_dim.InnerText = m.dimensions.X + " " + m.dimensions.Y;

                miasma.AppendChild(m_item);
                m_item.AppendChild(m_pos);
                m_item.AppendChild(m_dim);
            }

            gameGrid.InnerText = l.gameGrid.X + " " + l.gameGrid.Y;

            foreach (Ribbon r in l.ribbons)
            {
                XmlElement r_item = doc.CreateElement("Item");
                XmlElement r_path = doc.CreateElement("path");
                XmlElement r_start = doc.CreateElement("start");
                XmlElement r_end = doc.CreateElement("end");
                XmlElement r_loop = doc.CreateElement("loop");
                XmlElement r_color = doc.CreateElement("color");

                foreach (Vector2 v in r.path)
                {
                    r_path.InnerText = r_path.InnerText + v.X + " " + v.Y + "\r\n";
                }

                r_start.InnerText = r.start.ToString();
                r_end.InnerText = r.end.ToString();
                r_loop.InnerText = r.loop.ToString().ToLower();
                r_color.InnerText = r.color.ToString();

                ribbons.AppendChild(r_item);
                r_item.AppendChild(r_path);
                r_item.AppendChild(r_start);
                r_item.AppendChild(r_end);
                r_item.AppendChild(r_loop);
                r_item.AppendChild(r_color);
            }

            foreach (Hotspot c in l.cameras)
            {

            }

            foreach (RibbonBox rB in l.ribbonBoxes)
            {
                XmlElement rB_item = doc.CreateElement("Item");
                XmlElement rB_ID = doc.CreateElement("ribbonID");
                XmlElement rB_pos = doc.CreateElement("position");
                XmlElement rB_dim = doc.CreateElement("dimensions");
                XmlElement rB_rot = doc.CreateElement("rotation");
                XmlElement rB_flp = doc.CreateElement("flipped");

                rB_ID.InnerText = rB.ribbonID.ToString();
                rB_pos.InnerText = rB.position.ToString();
                rB_dim.InnerText = rB.dimensions.X + " " + rB.dimensions.Y;
                rB_rot.InnerText = rB.rotation.ToString();
                rB_flp.InnerText = rB.flipped.ToString().ToLower() ;

                ribbonBoxes.AppendChild(rB_item);
                rB_item.AppendChild(rB_ID);
                rB_item.AppendChild(rB_pos);
                rB_item.AppendChild(rB_dim);
                rB_item.AppendChild(rB_rot);
                rB_item.AppendChild(rB_flp);
            }

            foreach (Shooter s in l.shooters)
            {
                /*Something may be wrong here based on the pos/rPos difference.*/
                XmlElement s_item = doc.CreateElement("Item");
                XmlElement s_ID = doc.CreateElement("ribbonID");
                XmlElement s_pos = doc.CreateElement("position");
                XmlElement s_rPos = doc.CreateElement("ribbonPosition");
                XmlElement s_freq = doc.CreateElement("frequency");
                XmlElement s_rot = doc.CreateElement("rotation");
                XmlElement s_flp = doc.CreateElement("flipped");

                s_ID.InnerText = s.ribbonID.ToString();
                s_pos.InnerText = s.position.X + " " + s.position.Y;
                s_rPos.InnerText = s.ribbonPosition.ToString();
                s_freq.InnerText = s.frequency.ToString();
                s_rot.InnerText = s.rotation.ToString();
                if (s.flipped)
                    s_flp.InnerText = "true";
                else 
                    s_flp.InnerText = "false";

                shooters.AppendChild(s_item);
                s_item.AppendChild(s_ID);
                s_item.AppendChild(s_pos);
                s_item.AppendChild(s_rPos);
                s_item.AppendChild(s_freq);
                s_item.AppendChild(s_rot);
                s_item.AppendChild(s_flp);
            }

            foreach (Needle n in l.needles)
            {
                XmlElement n_item = doc.CreateElement("Item");
                XmlElement n_dim = doc.CreateElement("dimensions");
                XmlElement n_ID = doc.CreateElement("ribbonID");
                XmlElement n_pos = doc.CreateElement("position");
                XmlElement n_amp = doc.CreateElement("amplitude");
                XmlElement n_freq = doc.CreateElement("frequency");
                XmlElement n_rot = doc.CreateElement("rotation");

                n_dim.InnerText = n.dimensions.X + " " + n.dimensions.Y;
                n_ID.InnerText = n.ribbonID.ToString();
                n_pos.InnerText = n.position.X + " " + n.position.Y;
                n_amp.InnerText = n.amplitude.ToString();
                n_freq.InnerText = n.frequency.ToString();
                n_rot.InnerText = n.rotation.ToString();

                needles.AppendChild(n_item);
                n_item.AppendChild(n_dim);
                n_item.AppendChild(n_pos);
                n_item.AppendChild(n_amp);
                n_item.AppendChild(n_freq);
                n_item.AppendChild(n_rot);

            }

            foreach (Hook h in l.hooks)
            {
                XmlElement n_item = doc.CreateElement("Item");
                XmlElement n_dim = doc.CreateElement("dimensions");
                XmlElement n_ID = doc.CreateElement("ribbonID");
                XmlElement n_pos = doc.CreateElement("ribbonPosition");
                XmlElement n_flp = doc.CreateElement("flipped");

                n_ID.InnerText = h.ribbonID.ToString();
                n_pos.InnerText = h.ribbonPosition.ToString();
                n_dim.InnerText = h.dimensions.X + " " + h.dimensions.Y;
                n_flp.InnerText = h.flipped.ToString().ToLower();

                hooks.AppendChild(n_item);
                n_item.AppendChild(n_ID);
                n_item.AppendChild(n_pos);
                n_item.AppendChild(n_dim);
                n_item.AppendChild(n_flp);
            }

            foreach (SaveRock sR in l.saveRocks)
            {
                XmlElement sR_item = doc.CreateElement("Item");
                XmlElement sR_pos = doc.CreateElement("position");
                XmlElement sR_endFlag = doc.CreateElement("endFlag");

                sR_pos.InnerText = sR.position.X + " " + sR.position.Y;
                sR_endFlag.InnerText = sR.endFlag.ToString().ToLower();
            }

            asset.AppendChild(level);
            asset.AppendChild(seamstress);
            asset.AppendChild(ground);
            asset.AppendChild(miasma);
            asset.AppendChild(gameGrid);
            asset.AppendChild(ribbons);
            asset.AppendChild(cameras);
            asset.AppendChild(ribbonBoxes);
            asset.AppendChild(shooters);
            asset.AppendChild(needles);
            asset.AppendChild(hooks);
            asset.AppendChild(telescopingBlocks);
            asset.AppendChild(flipbars);
            asset.AppendChild(rotatingPlatforms);
            asset.AppendChild(saveRocks);
            asset.AppendChild(doc.CreateElement("decorations"));
            asset.AppendChild(doc.CreateElement("collectables"));
            asset.AppendChild(doc.CreateElement("gems"));

            XmlTextWriter writer = new XmlTextWriter(path, null);
            writer.Formatting = Formatting.Indented;
            doc.Save(writer);

        }
    }
}
