using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace LineOfSight
{
    internal class ShortcutDisplay : CosmeticSprite
    {
        private LOSController owner;
        private Player Ply => (room.game.Players.Count > 0) ? room.game.Players[0].realizedCreature as Player : null;

        private float _alpha;
        private float _lastAlpha;

        public ShortcutDisplay(LOSController owner)
        {
            this.owner = owner;
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 drawPos = Vector2.Lerp(lastPos, pos, timeStacker);
            float drawAlpha = Mathf.Lerp(_lastAlpha, _alpha, timeStacker);
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].SetPosition(drawPos - camPos);
                sLeaser.sprites[i].alpha = drawAlpha;
                sLeaser.sprites[i].isVisible = !owner.hideAllSprites;
            }

            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[0] = new FSprite("Futile_White")
            {
                shader = rCam.game.rainWorld.Shaders["FlatLight"],
                scaleX = 6f,
                scaleY = 6f,
                color = new Color(1f, 1f, 1f, 0.2f)
            };
            sLeaser.sprites[1] = new FSprite("ShortcutArrow")
            {
                rotation = 180f,
                anchorY = 1f
            };

            AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("HUD"));
        }

        private FieldInfo _RainWorldGame_updateShortCut = typeof(RainWorldGame).GetField("updateShortCut", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public override void Update(bool eu)
        {
            base.Update(eu);

            Player ply = Ply;

            _lastAlpha = _alpha;

            // Find the player's shortcut vessel
            ShortcutHandler.ShortCutVessel plyVessel = null;
            foreach (ShortcutHandler.ShortCutVessel vessel in room.game.shortcuts.transportVessels)
            {
                if (vessel.creature == ply && vessel.room == room.abstractRoom)
                {
                    plyVessel = vessel;
                    break;
                }
            }

            if (plyVessel != null)
            {
                // Find the player's position in a shortcut
                Vector2 scPos = room.MiddleOfTile(plyVessel.pos);
                Vector2 lastScPos = room.MiddleOfTile(plyVessel.lastPos);
                //int update = (int)_RainWorldGame_updateShortCut.GetValue(room.game);
                //pos = Vector2.Lerp(lastScPos, scPos, (update + 1f) / 3f);
                lastPos = scPos;
                pos = scPos;
                _alpha = Mathf.Min(_alpha + 0.2f, 1f);
            }
            else
            {
                // Fade out when not in use
                _alpha = Mathf.Max(_alpha - 0.2f, 0f);
            }
        }
    }
}
