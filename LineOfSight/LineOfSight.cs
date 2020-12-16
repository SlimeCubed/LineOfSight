using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace LineOfSight
{
    public partial class LineOfSightMod : Partiality.Modloader.PartialityMod
    {
        public static bool classic = false;

        public LineOfSightMod()
        {
            ModID = "Line of Sight";
            Version = "0.1";
            author = "Slime_Cubed";
        }

        // FOR MULTIPLE PLAYERS:
        // Same mesh generation process repeated for all players
        // The shader process goes like this:
        // 1 - Draw LOS mesh, set bit 1 of the stencil mask
        // 2 - Draw fullscreen quad, if bit 1 is not set then set bit 0
        // 3 - Draw fullscreen quad, unset bit 1
        // 4 - Repeat steps 1-3 for each player
        // 5 - Draw fullscreen quad, if bit 0 is set then draw LOS blocker

        public override void OnEnable()
        {
            base.OnEnable();

            On.RoomCamera.DrawUpdate += RoomCamera_DrawUpdate;
            On.Room.Loaded += Room_Loaded;
        }

        private FieldInfo _RoomCamera_spriteLeasers = typeof(RoomCamera).GetField("spriteLeasers", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private void RoomCamera_DrawUpdate(On.RoomCamera.orig_DrawUpdate orig, RoomCamera self, float timeStacker, float timeSpeed)
        {
            orig(self, timeStacker, timeSpeed);
            List<RoomCamera.SpriteLeaser> sLeasers = (List<RoomCamera.SpriteLeaser>)_RoomCamera_spriteLeasers.GetValue(self);

            LOSController.hackToDelayDrawingUntilAfterTheLevelMoves = true;
            for (int i = 0; i < sLeasers.Count; i++)
            {
                if (!(sLeasers[i].drawableObject is LOSController)) continue;
                sLeasers[i].Update(timeStacker, self, Vector2.zero);
                if (sLeasers[i].deleteMeNextFrame)
                    sLeasers.RemoveAt(i);
            }
            LOSController.hackToDelayDrawingUntilAfterTheLevelMoves = false;
        }

        private void Room_Loaded(On.Room.orig_Loaded orig, Room self)
        {
            if (self.game != null)
            {
                self.AddObject(new LOSController(self));
                self.AddObject(new ShortcutDisplay());
            }
            orig(self);
        }
    }
}
