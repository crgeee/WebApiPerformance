namespace WebApiPerformance.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestViewModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value1 = c.String(),
                        Value2 = c.String(),
                        Value3 = c.String(),
                        Value4 = c.String(),
                        Value5 = c.String(),
                        Value6 = c.String(),
                        Value7 = c.String(),
                        Value8 = c.String(),
                        Value9 = c.String(),
                        Value10 = c.String(),
                        Value11 = c.String(),
                        Value12 = c.String(),
                        Value13 = c.String(),
                        Value14 = c.String(),
                        Value15 = c.String(),
                        Value16 = c.String(),
                        Value17 = c.String(),
                        Value18 = c.String(),
                        Value19 = c.String(),
                        Value20 = c.String(),
                        Value21 = c.String(),
                        Value22 = c.String(),
                        Value23 = c.String(),
                        Value24 = c.String(),
                        Value25 = c.String(),
                        Value26 = c.String(),
                        Value27 = c.String(),
                        Value28 = c.String(),
                        Value29 = c.String(),
                        Value30 = c.String(),
                        Value31 = c.String(),
                        Value32 = c.String(),
                        Value33 = c.String(),
                        Value34 = c.String(),
                        Value35 = c.String(),
                        Value36 = c.String(),
                        Value37 = c.String(),
                        Value38 = c.String(),
                        Value39 = c.String(),
                        Value40 = c.String(),
                        Value41 = c.String(),
                        Value42 = c.String(),
                        Value43 = c.String(),
                        Value44 = c.String(),
                        Value45 = c.String(),
                        Value46 = c.String(),
                        Value47 = c.String(),
                        Value48 = c.String(),
                        Value49 = c.String(),
                        Value50 = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TestViewModel");
        }
    }
}
