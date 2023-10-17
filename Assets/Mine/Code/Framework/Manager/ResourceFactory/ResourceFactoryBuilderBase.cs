namespace Mine.Code.Framework.Manager.ResourceFactory
{
    public class ResourceFactoryBuilderBase<T> where T : ResourceFactoryBuilderBase<T> 
    {
        protected bool isInject;
        protected bool isAddressable;

#if VCONTAINER_SUPPORT
        public T ByInject
        {
            get
            {
                isInject = true;

                return this as T;
            }
        }
#endif

#if ADDRESSABLE_SUPPORT
        public T ByAddressable
        {
            get
            {
                isAddressable = true;

                return this as T;
            }
        }
#endif
    }
}