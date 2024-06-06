using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace ttki
{
	public static class Codec
	{
		public static List<WorkArea> DecodeWorkAreas(JArray json)
		{
			List<WorkArea> areas = new List<WorkArea>();

			foreach (JObject j in json)
			{
				areas.Add(DecodeWorkArea(j));
			}

			return areas;
		}

		static WorkArea DecodeWorkArea(JObject json)
		{
			WorkArea area = new WorkArea();
			area.Uuid = json.Value<string>("uuid");
			area.Name = json.Value<string>("name");

			return area;
		}

		public static List<Work> DecodeWorks(JArray json)
		{
            List<Work> works = new List<Work>();

            foreach (JObject j in json)
            {
                works.Add(DecodeWork(j));
            }

            return works;
        }

		static Work DecodeWork(JObject json)
		{
            Work work = new Work();
            work.WorkArea = json.Value<string>("workarea_uuid");
            work.Start = json.Value<string>("start_time");
            work.Stop = json.Value<string>("stop_time");

            return work;
        }
	}
}
