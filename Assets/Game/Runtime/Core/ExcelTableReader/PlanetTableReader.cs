using System;
using System.Data;
using Game.Runtime.Core.Attributes;
using Game.Runtime.Data;

namespace Game.Runtime.Core.ExcelTableReader
{
    [ExcelSheet("Planet")]
    public class PlanetTableReader : IExcelTableReader
    {
        private const int DataStartRow = 3; //0是表头，1是类型，2是说明 


        public void Read(DataTable table, ExcelTableContext context)
        {
            ColumnSchema schema = ColumnSchema.Build(table);
            for (var i = DataStartRow; i < table.Rows.Count; i++)
            {
                var row = new SmartRow(table.Rows[i], schema);

                // 跳过空行
                if (row.IsEmpty("PlanetId"))
                    continue;

                var id = row.GetInt("PlanetId");

                if (context.planets.ContainsKey(id))
                    throw new Exception($"星球数据重复 planet_id={id} ");


                var planet = new PlanetData()
                {
                    id = id,
                    name = row.GetString("Name"),
                    description = row.GetString("Description"),
                    planetRequire = row.GetString("PlanetRequire"),
                    iconName = row.GetString("PlanetIcon"),
                };

                context.planets[planet.id] = planet;
            }
        }
    }
}