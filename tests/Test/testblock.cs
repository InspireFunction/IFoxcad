﻿namespace test;
public class TestBlock
{
    //块定义
    [CommandMethod("blockdef")]
    public void BlockDef()
    {
        using var tr = new DBTrans();
        //var line = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
        tr.BlockTable.Add("test",
            btr =>
            {
                btr.Origin = new Point3d(0, 0, 0);
            },
            () => //图元
            {
                return new List<Entity> { new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0)) };
            },
            () => //属性定义
            {
                var id1 = new AttributeDefinition() { Position = new Point3d(0, 0, 0), Tag = "start", Height = 0.2 };
                var id2 = new AttributeDefinition() { Position = new Point3d(1, 1, 0), Tag = "end", Height = 0.2 };
                return new List<AttributeDefinition> { id1, id2 };
            }
        );
        //ObjectId objectId = tr.BlockTable.Add("a");//新建块
        //objectId.GetObject<BlockTableRecord>().AddEntity();//测试添加空实体
        tr.BlockTable.Add("test1",
        btr =>
        {
            btr.Origin = new Point3d(0, 0, 0);

        },
        () =>
        {
            return new List<Entity> { new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0)) ,
            new DBText{ Position = new Point3d(0,0,0),
            TextString = "123"
            } };
        });
    }
    //修改块定义
    [CommandMethod("blockdefchange")]
    public void BlockDefChange()
    {
        using var tr = new DBTrans();
        //var line = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
        //tr.BlockTable.Change("test", btr =>
        //{
        //    btr.Origin = new Point3d(5, 5, 0);
        //    btr.AddEntity(new Circle(new Point3d(0, 0, 0), Vector3d.ZAxis, 2));
        //    btr.GetEntities<BlockReference>()
        //        .ToList()
        //        .ForEach(e => e.Flush()); //刷新块显示

        //});

        tr.BlockTable.Change("test", btr =>
        {
            foreach (var id in btr)
            {
                var ent = tr.GetObject<Entity>(id);
                using (ent.ForWrite())
                {
                    if (ent is Dimension dBText)
                    {
                        dBText.DimensionText = "234";
                        dBText.RecomputeDimensionBlock(true);
                    }
                }
                
            }
        });
        tr.Editor.Regen();
    }

    [CommandMethod("insertblockdef")]
    public void InsertBlockDef()
    {
        using var tr = new DBTrans();
        var line1 = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
        var line2 = new Line(new Point3d(0, 0, 0), new Point3d(-1, 1, 0));
        var att1 = new AttributeDefinition() { Position = new Point3d(10, 10, 0), Tag = "tagTest1", Height = 1, TextString = "valueTest1" };
        var att2 = new AttributeDefinition() { Position = new Point3d(10, 12, 0), Tag = "tagTest2", Height = 1, TextString = "valueTest2" };
        tr.BlockTable.Add("test1", line1, line2, att1, att2);


        var ents = new List<Entity>();
        var line5 = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
        var line6 = new Line(new Point3d(0, 0, 0), new Point3d(-1, 1, 0));
        ents.Add(line5);
        ents.Add(line6);
        tr.BlockTable.Add("test44", ents);


        var line3 = new Line(new Point3d(5, 5, 0), new Point3d(6, 6, 0));
        var line4 = new Line(new Point3d(5, 5, 0), new Point3d(-6, 6, 0));
        var att3 = new AttributeDefinition() { Position = new Point3d(10, 14, 0), Tag = "tagTest3", Height = 1, TextString = "valueTest3" };
        var att4 = new AttributeDefinition() { Position = new Point3d(10, 16, 0), Tag = "tagTest4", Height = 1, TextString = "valueTest4" };
        tr.BlockTable.Add("test2", new List<Entity> { line3, line4 }, new List<AttributeDefinition> { att3, att4 });
        //tr.CurrentSpace.InsertBlock(new Point3d(4, 4, 0), "test1"); // 测试默认
        //tr.CurrentSpace.InsertBlock(new Point3d(4, 4, 0), "test2");
        //tr.CurrentSpace.InsertBlock(new Point3d(4, 4, 0), "test3"); //测试插入不存在的块定义
        //tr.CurrentSpace.InsertBlock(new Point3d(0, 0, 0), "test1", new Scale3d(2)); // 测试放大2倍
        //tr.CurrentSpace.InsertBlock(new Point3d(4, 4, 0), "test1", new Scale3d(2), Math.PI / 4); // 测试放大2倍,旋转45度

        var def1 = new Dictionary<string, string>
            {
                { "tagTest1", "1" },
                { "tagTest2", "2" }
            };
        tr.CurrentSpace.InsertBlock(new Point3d(0, 0, 0), "test1", atts: def1);
        var def2 = new Dictionary<string, string>
            {
                { "tagTest3", "1" },
                { "tagTest4", "" }
            };
        tr.CurrentSpace.InsertBlock(new Point3d(10, 10, 0), "test2", atts: def2);
        tr.CurrentSpace.InsertBlock(new Point3d(-10, 0, 0), "test44");
    }

    [CommandMethod("testblocknullbug")]
    public void TestBlockNullBug()
    {
        using var tr = new DBTrans();

        var ents = new List<Entity>();
        var line5 = new Line(new Point3d(0, 0, 0), new Point3d(1, 1, 0));
        var line6 = new Line(new Point3d(0, 0, 0), new Point3d(-1, 1, 0));
        ents.Add(line5);
        ents.Add(line6);
        tr.BlockTable.Add("test44", ents);
        tr.CurrentSpace.InsertBlock(new Point3d(0, 0, 0), "test44");
    }

    [CommandMethod("test_block_file")]
    public void TestBlockFile()
    {
        var tr = new DBTrans();
        var id = tr.BlockTable.GetBlockFrom(@"C:\Users\vic\Desktop\test.dwg",false);
        tr.CurrentSpace.InsertBlock(Point3d.Origin, id);
    }


    [CommandMethod("testclip")]
    public void TestClipBlock()
    {
        using var tr = new DBTrans();
        tr.BlockTable.Add("test1",
            btr =>
            {
                btr.Origin = new Point3d(0, 0, 0);
                btr.AddEntity(new Line(new Point3d(0, 0, 0), new Point3d(10, 10, 0)),
                    new Line(new Point3d(10, 10, 0), new Point3d(10, 0, 0))
                    );
            }
            );
        //tr.BlockTable.Add("hah");
        var id = tr.CurrentSpace.InsertBlock(new Point3d(0, 0, 0), "test1");
        var bref = tr.GetObject<BlockReference>(id);
        var pts = new List<Point3d> { new Point3d(3, 3, 0), new Point3d(7, 3, 0), new Point3d(7, 7, 0), new Point3d(3, 7, 0) };
        bref.ClipBlockRef(pts);

        var id1 = tr.CurrentSpace.InsertBlock(new Point3d(20, 20, 0), "test1");
        var bref1 = tr.GetObject<BlockReference>(id);

        bref1.ClipBlockRef(new Point3d(13, 13, 0), new Point3d(17, 17, 0));
    }

    /// <summary>
    /// 给用户的测试程序，不知道对错
    /// </summary>
    [CommandMethod("test_block_ej")]
    public void EJ()
    {
        using (var tr = new DBTrans())
        {

            //Point3d.Origin.AddBellowToModelSpace(100, 100, 5, 3, 30);//画波纹管

            //Database db2 = new Database(false, true);
            //string fullFileName = @".\MyBlockDwgFile\001.dwg";
            //db2.ReadDwgFile(fullFileName, System.IO.FileShare.Read, true, null);
            //db2.CloseInput(true);
            //string blockName = "test";
            //if (!tr.BlockTable.Has(blockName))
            //{
            //    //tr.Database.Insert(blockName, db2, false);//插入块
            //    db.Insert(blockName, db2, false);

            //}

            string fullFileName = @"C:\Users\vic\Desktop\001.dwg";
            var blockdef = tr.BlockTable.GetBlockFrom(fullFileName, false);

            tr.Database.Clayer = tr.LayerTable["0"];//当前图层切换为0图层
            tr.LayerTable.Change(tr.Database.Clayer, ltr =>
            {
                ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 2); //ColorMethod.ByAci可以让我们使用AutoCAD ACI颜色索引……这里为2（表示黄色）
        });

            ObjectId id = tr.ModelSpace.InsertBlock(Point3d.Origin, blockdef);//插入块参照





            var entTest = tr.GetObject<BlockReference>(id);
            entTest.Draw();
    
        }

        using var tr2 = new DBTrans();
        PromptEntityOptions PEO = new PromptEntityOptions("\n请选择一个块");
        PEO.SetRejectMessage("\n对象必须是块");
        PEO.AddAllowedClass(typeof(BlockReference), true);

        PromptEntityResult PER = Env.Editor.GetEntity(PEO);
        if (PER.Status != PromptStatus.OK)
        {
            return;
        }

        var Bref = tr2.GetObject<BlockReference>(PER.ObjectId);
        //var BTR = tr.GetObject<BlockTableRecord>(Bref.BlockTableRecord, OpenMode.ForWrite);
        ////如果知道块名字BTRName
        //BlockTableRecord BTR = tr.GetObject<BlockTableRecord>(tr.BlockTable[blockName], OpenMode.ForWrite);

        var btr = tr2.BlockTable[Bref.Name];

        tr2.BlockTable.Change(btr, ltr =>
        {

            foreach (ObjectId OID in ltr)
            {
                var Ent = tr2.GetObject<Entity>(OID);
                using (Ent.ForWrite())
                {
                    if (Ent is MText mText)
                    {
                        switch(mText.Text)
                        {
                            case "$$A":
                                mText.Contents = "hahaha";
                                break;
                            case "$$B":
                                ;
                                break;
                            default:
                                ;
                                break;
                        }

                    };
                    if (Ent is DBText dBText) { dBText.TextString = "haha"; };
                    if (Ent is Dimension dimension)
                    {
                        switch (dimension.DimensionText)
                        {
                            case "$$pipeLen":
                                dimension.DimensionText = "350";
                                dimension.RecomputeDimensionBlock(true);
                                break;
                            default:
                                break;
                        }
                    };
                }
              

            }
           
        });


        tr2.Editor.Regen();


    }

    [CommandMethod("W_KSZK")]
    public void QuickBlockDef()
    {
        //Database db = HostApplicationServices.WorkingDatabase;
        Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        PromptSelectionOptions promptOpt = new PromptSelectionOptions
        {
            MessageForAdding = "请选择需要快速制作块的对象"
        };
        string blockName = "W_BLOCK_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //var rss = ed.GetSelection(promptOpt);
        var rss = Env.Editor.GetSelection(promptOpt);
        using (var tr = new DBTrans())
        {
            if (rss.Status == PromptStatus.OK)
            {
                //SelectionSet ss = rss.Value;
                //ObjectId[] ids = ss.GetObjectIds();
                //var ents = new List<KeyValuePair<Entity, long>>();
                //var extents = new Extents3d();
                //foreach (var id in ids)
                //{
                //    Entity ent = tr.GetObject<Entity>(id);
                //    if (ent == null)
                //        continue;
                //    try
                //    {
                //        extents.AddExtents(ent.GeometricExtents);
                //        var order = id.Handle.Value;
                //        var newEnt = ent.Clone() as Entity;
                //        ents.Add(new KeyValuePair<Entity, long>(newEnt, order));
                //        ent.UpgradeOpen();
                //        ent.Erase();
                //        ent.DowngradeOpen();
                //    }
                //    catch (System.Exception exc)
                //    {
                //        ed.WriteMessage(exc.Message);
                //    }
                //}
                //ents = ents.OrderBy(x => x.Value).ToList();
                var ents = rss.Value.GetEntities<Entity>();
                //ents.ForEach(ent => extents.AddExtents(ent.GeometricExtents));
                var extents = ents.GetExtents();

                Point3d pt = extents.MinPoint;
                Matrix3d matrix = Matrix3d.Displacement(Point3d.Origin - pt);
                //var newEnts = new List<Entity>();
                //foreach (var ent in ents)
                //{
                //    var newEnt = ent.Key;
                //    newEnt.TransformBy(matrix);
                //    newEnts.Add(newEnt);
                //}
                //if (tr.BlockTable.Has(blockName))
                //{
                //    Application.ShowAlertDialog(Environment.NewLine + "块名重复，程序退出！");
                //    return;
                //}
                ents.ForEach(ent =>
                    ent.ForWrite(e => e.TransformBy(matrix)));
                //var newents = ents.Select(ent =>
                //{
                //    var maping = new IdMapping();
                //    return ent.DeepClone(ent, maping, true) as Entity;
                //});
                var newents = ents.Select(ent => ent.Clone() as Entity);

                //ents.ForEach(ent => ent.ForWrite(e => e.Erase(true))); // 删除实体就会卡死，比较奇怪，估计是Clone()函数的问题
                // 经过测试不是删除的问题
                var btrId = tr.BlockTable.Add(blockName, newents);
                ents.ForEach(ent => ent.ForWrite(e => e.Erase(true)));
                var bId = tr.CurrentSpace.InsertBlock(pt, blockName);
                //tr.GetObject<Entity>(bId, OpenMode.ForWrite).Move(Point3d.Origin, Point3d.Origin);
                //var ed = Application.DocumentManager.MdiActiveDocument.Editor;
                //ed.Regen();
                //tr.Editor.Regen();
                // 调用regen() 卡死
            }
            //tr.Editor.Regen();
            //ed.Regen();
        }
        //using (var tr = new DBTrans())
        //{
        //    tr.CurrentSpace.InsertBlock(Point3d.Origin, blockName);
        //    tr.Editor.Regen();
        //}
    }

    [CommandMethod("testquickblockdef")]
    public void TestQuickBlockDef()
    {
        Database db = HostApplicationServices.WorkingDatabase;
        Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        PromptSelectionOptions promptOpt = new PromptSelectionOptions
        {
            MessageForAdding = "请选择需要快速制作块的对象"
        };
        string blockName = "W_BLOCK_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var rss = ed.GetSelection(promptOpt);
        //var rss = Env.Editor.GetSelection(promptOpt);
        if (rss.Status != PromptStatus.OK)
        {
            return;
        }

        using (var tr = db.TransactionManager.StartTransaction())
        {
            var ids = rss.Value.GetObjectIds();
            var bt = tr.GetObject(db.BlockTableId,OpenMode.ForRead) as BlockTable;
            var btr = new BlockTableRecord();
            btr.Name = blockName;
            foreach (var item in ids)
            {
                var ent = tr.GetObject(item,OpenMode.ForRead) as Entity;

                btr.AppendEntity(ent.Clone() as Entity);
                ent.ForWrite(e => e.Erase(true));
            }
            bt.UpgradeOpen();
            bt.Add(btr);
            tr.AddNewlyCreatedDBObject(btr,true);
            bt.DowngradeOpen();
        //    tr.Commit();
        //}

        //using (var tr1 = db.TransactionManager.StartTransaction())
        //{
            //var bt = tr1.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            var btr1 = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            var br = new BlockReference(Point3d.Origin, bt[blockName]);
            br.ScaleFactors = default;
            btr1.AppendEntity(br);
            tr.AddNewlyCreatedDBObject(br,true);
            btr1.DowngradeOpen();
            ed.Regen();
            tr.Commit();
        }
        //ed.Regen();

    }

    public void TestWblock()
    {
        Database db = new Database(false,true);
        var curdb = HostApplicationServices.WorkingDatabase;
        var opts = new PromptSelectionOptions();
        opts.MessageForAdding = "选择对象";
        var ss = Env.Editor.GetSelection(opts).Value;
        var ids = new ObjectIdCollection(ss.GetObjectIds());
        db = curdb.Wblock(ids, Point3d.Origin);
        db.SaveAs(@"c:\test.dwg", DwgVersion.Current);
    }

}