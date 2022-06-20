namespace Shooter.Utils.MVP
{
    public abstract class Presenter<M, V>
    {
        protected readonly M Model;
        protected readonly V View;

        public Presenter(M model, V view)
        {
            Model = model;
            View = view;
        }
    }
}
