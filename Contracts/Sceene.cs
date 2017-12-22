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

        protected IRenderScope scope;

        protected InputMask UiInput { get; private set; }

        protected Sceene(string sceeneName, IRenderer renderer, IUserInterface ui, ActionQueue actionQueue)
        {
            this.Name = sceeneName;
            this.Renderer = renderer;
            this.UI = ui;
            this.ActionQueue = actionQueue;
        }

        public virtual void Exiting()
        {
        }

        public abstract void Update(float timestep);

        public virtual void Activate(InputMask uiInput, InputMask[] inputMasks)
        {
            UiInput = uiInput;
            // Create a scope in the renderer and set it active. This allows the renderer to manage resources.
            this.scope = Renderer.ActivateScope(Name);
        }

        public virtual string[] DiagnosticsString()
        {
            return new string[0];
        }

        public abstract void Render(double gameTimeMsec);

        public virtual void Deactivate()
        {
            Renderer.DeactivateScope(Name);
        }

        public IBinding<ISprite> LoadSprite(string spriteName)
        {
            return scope.ResolveSprite(new ObjectBinding<ISprite>(spriteName));
        }

        public IFont LoadFont(string fontName)
        {
            return scope.LoadFont(fontName);
        }
    }
}
