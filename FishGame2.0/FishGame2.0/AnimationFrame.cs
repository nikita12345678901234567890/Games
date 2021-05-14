﻿using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class AnimationFrame
    {
        public Rectangle Frame;
        public Vector2 Origin;

        public AnimationFrame(Rectangle Frame, Vector2? Origin = null)
        {
            this.Frame = Frame;
            if (!Origin.HasValue)
            {
                this.Origin = Frame.Center.ToVector2();
            }
            else
            {
                this.Origin = Origin.Value;
            }
        }
    }
}