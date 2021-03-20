namespace Aetos.ComparisonGenerator
{
    internal partial class ObjectEqualsGenerator
    {
        public ObjectEqualsGenerator(
            SourceTypeInfo sourceTypeInfo)
            : base(
                sourceTypeInfo)
        {
            var options = sourceTypeInfo.GenerateOptions;

            this.CanDelegateToEquatable =
                sourceTypeInfo.IsEquatable || options.GenerateEquatable;

            this.CanDelegateToGenericComparable =
                sourceTypeInfo.IsGenericComparable || options.GenerateGenericComparable;

            this.CanDelegateToNonGenericComparable =
                sourceTypeInfo.IsNonGenericComparable || options.GenerateNonGenericComparable;

            this.CanDelegateToStructuralEquatable =
                sourceTypeInfo.IsStructuralEquatable || options.GenerateStructuralEquatable;

            this.CanDelegateToStructuralComparable =
                sourceTypeInfo.IsStructuralComparable || options.GenerateStructuralComparable;
        }

        public bool CanDelegateToEquatable { get; }

        public bool CanDelegateToGenericComparable { get; }

        public bool CanDelegateToNonGenericComparable { get; }

        public bool CanDelegateToStructuralEquatable { get; }

        public bool CanDelegateToStructuralComparable { get; }

        public bool DelegateToStructuralEquatable
        {
            get
            {
                return
                    !this.CanDelegateToEquatable &&
                    !this.CanDelegateToGenericComparable &&
                    !this.CanDelegateToNonGenericComparable &&
                    this.CanDelegateToStructuralEquatable;
            }
        }

        public bool DelegateToStructuralComparable
        {
            get
            {
                return
                    !this.CanDelegateToEquatable &&
                    !this.CanDelegateToGenericComparable &&
                    !this.CanDelegateToNonGenericComparable &&
                    !this.CanDelegateToStructuralEquatable &&
                    this.CanDelegateToStructuralComparable;
            }
        }
    }
}
