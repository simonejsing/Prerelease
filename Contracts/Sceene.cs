using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public abstract class Sceene : ISceene
    {
        protected IRenderer Renderer;
        protected IUserInterface UI;
        protected ActionQueue ActionQueue;
        protected string Name;

        private IRenderScope scope;

        protected Sceene(string sceeneName, IRenderer renderer, IUserInterface ui, ActionQueue actionQueue)
        {
            this.Name = sceeneName;
            this.Renderer = renderer;
            this.UI = ui;
            this.ActionQueue = actionQueue;
        }

        public abstract void Update(float timestep);

        public virtual void Activate(InputMask[] inputMasks)
        {
            // Create a scope in the renderer and set it active. This allows the renderer to manage resources.
            this.scope = Renderer.ActivateScope(Name);
        }

        public abstract void Render(double gameTimeMsec);

        public virtual void Deactivate()
        {
            Renderer.DeactivateScope(Name);
        }

        public ISprite LoadSprite(string spriteName)
        {
            return scope.LoadSprite(spriteName);
        }

        public IFont LoadFont(string fontName)
        {
            return scope.LoadFont(fontName);
        }
    }
}
