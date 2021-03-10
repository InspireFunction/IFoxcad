﻿using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Internal;

using IFoxCAD.Cad;
using Autodesk.AutoCAD.Colors;


namespace test
{
    public class Class1
    {
        [CommandMethod("dbtest")]
        public void Dbtest()
        {
            using var tr = new DBTrans();
            tr.Editor.WriteMessage("\n测试 Editor 属性是否工作！");
            tr.Editor.WriteMessage("\n----------开始测试--------------");
            tr.Editor.WriteMessage("\n测试document属性是否工作");
            if (tr.Document == Getdoc())
            {
                tr.Editor.WriteMessage("\ndocument 正常");
            }
            tr.Editor.WriteMessage("\n测试database属性是否工作");
            if (tr.Database == Getdb())
            {
                tr.Editor.WriteMessage("\ndatabase 正常");
            }

            Line line = new(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
            Circle circle = new(new Point3d(0, 0, 0), Vector3d.ZAxis, 2);
            var lienid = tr.AddEntity(line);
            var cirid = tr.AddEntity(circle);
            var linent = tr.GetObject<Line>(lienid); 
            var lineent = tr.GetObject<Circle>(cirid);
            var linee = tr.GetObject<Line>(cirid); //经测试，类型不匹配，返回null
            var dd = tr.GetObject<Circle>(lienid);
            List<DBObject> ds = new() { linee, dd };
        }

        [CommandMethod("layertest")]
        public void Layertest()
        {
            using var tr = new DBTrans();
            tr.LayerTable.Add("1");
            tr.LayerTable.Add("2", lt =>
            {
                lt.Color = Color.FromColorIndex(ColorMethod.ByColor, 1);
                lt.LineWeight = LineWeight.LineWeight030;

            });
            tr.LayerTable.Remove("3");
            tr.LayerTable.Change("4", lt =>
            {
                lt.Color = Color.FromColorIndex(ColorMethod.ByColor, 2);
            });
        }
        [CommandMethod("layerAdd1")]
        public void Layertest1()
        {
            using var tr = new DBTrans();
            tr.LayerTable.Add("test1", Color.FromColorIndex(ColorMethod.ByColor,1));
        }
        [CommandMethod("layerAdd2")]
        public void Layertest2()
        {
            using var tr = new DBTrans();
            tr.LayerTable.Add("test2", 2);
            //tr.LayerTable["3"] = new LayerTableRecord();
        }

        
        [CommandMethod("linedemo1")]
        public void addLine1()
        {
            using var tr = new DBTrans();
            //    tr.ModelSpace.AddEnt(line);
            //    tr.ModelSpace.AddEnts(line,circle);

            //    tr.PaperSpace.AddEnt(line);
            //    tr.PaperSpace.AddEnts(line,circle);

            // tr.addent(btr,line);
            // tr.addents(btr,line,circle);



            //    tr.BlockTable.Add(new BlockTableRecord(), line =>
            //    {
            //        line.
            //    });
            Line line = new(new Point3d(0,0,0),new Point3d(1,1,0));
            tr.AddEntity(line);
        }

        //块定义
        [CommandMethod("blockdef")]
        public void BlockDef()
        {
            using var tr = new DBTrans();
            //var line = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
            tr.BlockTable.Add("test", () =>
            {
                return new List<Entity> { new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0))};
            });
        }
        //修改块定义
        [CommandMethod("blockdefchange")]
        public void BlockDefChange()
        {
            using var tr = new DBTrans();
            //var line = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
            tr.BlockTable.Change("test", btr =>
            {
                btr.Origin = new Point3d(5, 5, 0);
                tr.AddEntity(new Circle(new Point3d(0, 0, 0), Vector3d.ZAxis, 2), btr);
                btr.Cast<ObjectId>()
                .Select(id => tr.GetObject<BlockReference>(id))
                .OfType<BlockReference>()
                .ToList()
                .ForEach(e => tr.Flush(e)); //刷新块显示
                
            });
        }

        [CommandMethod("PrintLayerName")]
        public void PrintLayerName()
        {
            using var tr = new DBTrans();
            foreach (var layerRecord in tr.LayerTable.GetRecords())
            {
                tr.Editor.WriteMessage(layerRecord.Name);
            }

        }

        public Database Getdb()
        {
            var db = Application.DocumentManager.MdiActiveDocument.Database;
            return db;
        }


        public Document Getdoc()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            return doc;
        }

    }
}
