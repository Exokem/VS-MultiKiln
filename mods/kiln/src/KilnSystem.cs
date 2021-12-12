using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kiln.kiln;
using Vintagestory.API.Common;

namespace kiln
{
	public class KilnSystem : ModSystem
	{
		public override void Start(ICoreAPI api)
		{
			base.Start(api);

			api.RegisterBlockClass("kiln", typeof(BlockKiln));
			api.RegisterBlockEntityClass("kiln", typeof(BlockEntityKiln));
		}
	}
}
