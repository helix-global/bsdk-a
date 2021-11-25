using System;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal class Asn1CertificateAuthorityInformationAccessCollection : Asn1ReadOnlyCollection<Asn1CertificateAuthorityInformationAccess>, IJsonSerializable
        {
        //protected class InternalDescriptor : PropertyDescriptor
        //    {
        //    private T Value { get; }
        //    private readonly Asn1ReadOnlyCollection<T> Owner;
        //    public InternalDescriptor(Asn1ReadOnlyCollection<T> owner, T value, String name)
        //        : base(name, new Attribute[0])
        //        {
        //        Value = value;
        //        Owner = owner;
        //        }

        //    #region M:CanResetValue(Object):Boolean
        //    /**
        //     * <summary>When overridden in a derived class, returns whether resetting an object changes its value.</summary>
        //     * <param name="component">The component to test for reset capability.</param>
        //     * <returns>true if resetting the component changes its value; otherwise, false.</returns>
        //     * */
        //    public override Boolean CanResetValue(Object component)
        //        {
        //        return false;
        //        }
        //    #endregion
        //    #region M:GetValue(Object):Object
        //    /**
        //     * <summary>When overridden in a derived class, gets the current value of the property on a component.</summary>
        //     * <param name="component">The component with the property for which to retrieve the value.</param>
        //     * <returns>The value of a property for a given component.</returns>
        //     * */
        //    public override Object GetValue(Object component)
        //        {
        //        return Value;
        //        }
        //    #endregion
        //    #region M:ResetValue(Object)
        //    /**
        //     * <summary>When overridden in a derived class, resets the value for this property of the component to the default value.</summary>
        //     * <param name="component">The component with the property value that is to be reset to the default value.</param>
        //     * */
        //    public override void ResetValue(Object component)
        //        {
        //        throw new NotImplementedException();
        //        }
        //    #endregion
        //    #region M:SetValue(Object,Object)
        //    /**
        //     * <summary>When overridden in a derived class, sets the value of the component to a different value.</summary>
        //     * <param name="component">The component with the property value that is to be set.</param>
        //     * <param name="value">The new value.</param>
        //     * */
        //    public override void SetValue(Object component, Object value)
        //        {
        //        throw new NotImplementedException();
        //        }
        //    #endregion
        //    #region M:ShouldSerializeValue(Object):Boolean
        //    /**
        //     * <summary>When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.</summary>
        //     * <param name="component">The component with the property to be examined for persistence.</param>
        //     * <returns>true if the property should be persisted; otherwise, false.</returns>
        //     * */
        //    public override Boolean ShouldSerializeValue(Object component)
        //        {
        //        return false;
        //        }
        //    #endregion

        //    public override Type ComponentType { get { return typeof(Asn1ReadOnlyCollection<T>); }}
        //    public override Boolean IsReadOnly { get { return true; }}
        //    public override Type PropertyType  { get { return (Value != null) ? Value.GetType() : typeof(Object); }}
        //    public override String DisplayName { get { return Owner.GetDisplayName(Name, Value); }}

        //    /**
        //     * <summary>Returns a string that represents the current object.</summary>
        //     * <returns>A string that represents the current object.</returns>
        //     * <filterpriority>2</filterpriority>
        //     * */
        //    public override String ToString()
        //        {
        //        return Name;
        //        }
        //    }

        public Asn1CertificateAuthorityInformationAccessCollection(IList<Asn1CertificateAuthorityInformationAccess> source)
            : base(source)
            {
            }

        public Asn1CertificateAuthorityInformationAccessCollection(IEnumerable<Asn1CertificateAuthorityInformationAccess> source)
            : this(source.ToList())
            {
            }

        //protected override PropertyDescriptorCollection EnsureOverride()
        //    {
        //    var r = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
        //    foreach (var i in Items)
        //        {
        //        r.Add(new X509CertificateAuthorityInformationAccessPropertyDescriptor(
        //            i,
        //            $"[{i}]"));
        //        }
        //    return r;
        //    }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"{Resources.Count} = {Count}";
            }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartArray();
            foreach (var i in Items) {
                i.WriteJson(writer, serializer);
                }
            writer.WriteEndArray();
            }
        }
    }