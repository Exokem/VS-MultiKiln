using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace kiln.kiln
{
	public class BlockKiln : Block
	{
		private WorldInteraction fuel, ignite;

		public override void OnLoaded(ICoreAPI api)
		{
			base.OnLoaded(api);

			// RETRIEVE ALLOWED FUEL ITEMS

			List<ItemStack> fuelItems = new List<ItemStack>();

			foreach (KeyValuePair<string, JsonObject> entry in Attributes["fuelTypes"].AsObject<Dictionary<string, JsonObject>>())
			{
				Item item = api.World.GetItem(new AssetLocation(entry.Key));
				Block block;

				// Prefer items, fallback to blocks

				if (item != null)
					fuelItems.Add(new ItemStack(item));
				
				else if ((block = api.World.GetBlock(new AssetLocation(entry.Key))) != null)
					fuelItems.Add(new ItemStack(block));
			}

			// DEFINE INTERACTIONS

			fuel = new WorldInteraction()
			{
				MouseButton = EnumMouseButton.Right,
				Itemstacks = fuelItems.ToArray(),
				ActionLangCode = "blockhelp-bloomery-fuel"
			};

			ignite = new WorldInteraction()
			{
				MouseButton = EnumMouseButton.Right,
				Itemstacks = new ItemStack[] { new ItemStack(api.World.GetBlock(new AssetLocation("torch-up"))) },
				HotKeyCode = "sneak",
				ActionLangCode = "blockhelp-bloomery-ignite"
			};
		}

		public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer player, BlockSelection blocks)
		{
			// Retrieve the block entity and attempt to notify it of the block interaction

			BlockEntityKiln ben = world.BlockAccessor.GetBlockEntity(blocks.Position) as BlockEntityKiln;

			if (ben == null)
				return base.OnBlockInteractStart(world, player, blocks);

			ben.ReceiveInteraction(player);
			(player as IClientPlayer)?.TriggerFpAnimation(EnumHandInteract.BlockInteract);

			return true;
		}

		public override EnumIgniteState OnTryIgniteBlock(EntityAgent byEntity, BlockPos pos, float secondsIgniting)
		{
			// TODO: determine if ignition is allowed from block entity state

			if (true)
				return EnumIgniteState.NotIgnitablePreventDefault;

			return 3 < secondsIgniting ? EnumIgniteState.IgniteNow : EnumIgniteState.Ignitable;
		}

		public override void OnTryIgniteBlockOver(EntityAgent byEntity, BlockPos pos, float secondsIgniting, ref EnumHandling handling)
		{
			// TODO: defer to block entity

			base.OnTryIgniteBlockOver(byEntity, pos, secondsIgniting, ref handling);
		}

		public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection,
			IPlayer forPlayer)
		{
			// TODO: output fuel or ignite based on block entity state

			return base.GetPlacedBlockInteractionHelp(world, selection, forPlayer);
		}
	}

	public class BlockEntityKiln : BlockEntityContainer
	{
		private InventoryGeneric inv;
		private ICoreClientAPI capi;
		private MeshData fullMesh;
		
		public override InventoryBase Inventory => inv;
		public override string InventoryClassName => "kiln";

		public BlockEntityKiln()
		{
			inv = new InventoryGeneric(1, null, null);
		}

		public override void Initialize(ICoreAPI api)
		{
			base.Initialize(api);

			capi = api as ICoreClientAPI;

			RegisterGameTickListener(Update, 500);
		}

		public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
		{
			if (inv[0].Empty)
				return false;

			if (fullMesh == null)
				capi.Tesselator.TesselateShape(Block, Api.Assets.TryGet("kiln:shapes/block/kiln/full.json").ToObject<Shape>(), out fullMesh);

			mesher.AddMeshData(fullMesh);

			return true;
		}

		public void Update(float delta)
		{
			
		}

		public void ReceiveInteraction(IPlayer player)
		{
			// TODO: Handle interaction cases

			if (!StructureFormed())
			{
				
			}
		}

		bool StructureFormed()
		{
			// TODO: determine whether the structure (TBI) is fully formed

			return true;
		}
	}
}
