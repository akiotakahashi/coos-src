/*  
 * PERWAPI - An API for Reading and Writing PE Files
 * 
 * Copyright (c) Diane Corney, Queensland University of Technology, 2004.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the PERWAPI Copyright as included with this
 * distribution in the file PERWAPIcopyright.rtf.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY as is explained in the copyright notice.
 *
 * The author may be contacted at d.corney@qut.edu.au
 * 
 */

using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Security.Permissions;
//using PERWAPI.PDB;

namespace PERWAPI {

    /// <summary>
    /// Diagnostic
    /// </summary>
    public class Diag {
        /// <summary>
        /// Flag for diagnostic output.
        /// </summary>
        public static readonly bool DiagOn = false;
    }

    /// <summary>
    /// Facilities for outputting hexadecimal strings
    /// </summary>
    public class Hex {
        readonly static char[] hexDigit = {'0','1','2','3','4','5','6','7',
                                        '8','9','A','B','C','D','E','F'};
        readonly static uint[] iByteMask = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };
        readonly static ulong[] lByteMask = {0x00000000000000FF, 0x000000000000FF00, 
                                          0x0000000000FF0000, 0x00000000FF000000,
                                          0x000000FF00000000, 0x0000FF0000000000,
                                          0x00FF000000000000, 0xFF00000000000000 };
        readonly static uint nibble0Mask = 0x0000000F;
        readonly static uint nibble1Mask = 0x000000F0;

        /// <summary>
        /// Derives a hexademical string for a byte value
        /// </summary>
        /// <param name="b">the byte value</param>
        /// <returns>hex string for the byte value</returns>
        public static String Byte(int b)
        {
            char[] str = new char[2];
            uint num = (uint)b;
            uint b1 = num & nibble0Mask;
            uint b2 = (num & nibble1Mask) >> 4;
            str[0] = hexDigit[b2];
            str[1] = hexDigit[b1];
            return new String(str);
        }

        /// <summary>
        /// Derives a hexademical string for a short value
        /// </summary>
        /// <param name="b">the short value</param>
        /// <returns>hex string for the short value</returns>
        public static String Short(int b)
        {
            char[] str = new char[4];
            uint num1 = (uint)b & iByteMask[0];
            uint num2 = ((uint)b & iByteMask[1]) >> 8;
            uint b1 = num1 & nibble0Mask;
            uint b2 = (num1 & nibble1Mask) >> 4;
            uint b3 = num2 & nibble0Mask;
            uint b4 = (num2 & nibble1Mask) >> 4;
            str[0] = hexDigit[b4];
            str[1] = hexDigit[b3];
            str[2] = hexDigit[b2];
            str[3] = hexDigit[b1];
            return new String(str);
        }

        /// <summary>
        /// Derives a hexademical string for an int value
        /// </summary>
        /// <param name="val">the int value</param>
        /// <returns>hex string for the int value</returns>
        public static String Int(int val)
        {
            char[] str = new char[8];
            uint num = (uint)val;
            int strIx = 7;
            for(int i=0; i < iByteMask.Length; i++) {
                uint b = num & iByteMask[i];
                b >>= (i*8);
                uint b1 = b & nibble0Mask;
                uint b2 = (b & nibble1Mask) >> 4;
                str[strIx--] = hexDigit[b1];
                str[strIx--] = hexDigit[b2];
            }
            return new String(str);
        }

        /// <summary>
        /// Derives a hexademical string for an unsigned int value
        /// </summary>
        /// <param name="num">the unsigned int value</param>
        /// <returns>hex string for the unsigned int value</returns>
        public static String Int(uint num)
        {
            char[] str = new char[8];
            int strIx = 7;
            for(int i=0; i < iByteMask.Length; i++) {
                uint b = num & iByteMask[i];
                b >>= (i*8);
                uint b1 = b & nibble0Mask;
                uint b2 = (b & nibble1Mask) >> 4;
                str[strIx--] = hexDigit[b1];
                str[strIx--] = hexDigit[b2];
            }
            return new String(str);
        }

        /// <summary>
        /// Derives a hexademical string for a long value
        /// </summary>
        /// <param name="lnum">the long value</param>
        /// <returns>hex string for the long value</returns>
        public static String Long(long lnum)
        {
            ulong num = (ulong)lnum;
            return Long(num);
        }

        /// <summary>
        /// Derives a hexademical string for an unsigned long value
        /// </summary>
        /// <param name="num">the unsigned long value</param>
        /// <returns>hex string for the unsigned long value</returns>
        public static String Long(ulong num)
        {
            char[] str = new char[16];
            int strIx = 15;
            for(int i=0; i < lByteMask.Length; i++) {
                ulong b = num & lByteMask[i];
                b >>= (i*8);
                ulong b1 = b & nibble0Mask;
                ulong b2 = (b & nibble1Mask) >> 4;
                str[strIx--] = hexDigit[b1];
                str[strIx--] = hexDigit[b2];
            }
            return new String(str);
        }

    }

    /// <summary>
    /// Exception for features yet to be implemented
    /// </summary>
    public class NotYetImplementedException : System.Exception {
        public NotYetImplementedException(string msg)
            : base(msg + " Not Yet Implemented")
        {
        }
    }

    /// <summary>
    /// Error in a type signature
    /// </summary>
    public class TypeSignatureException : System.Exception {
        public TypeSignatureException(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// Error with a CIL instruction
    /// </summary>
    public class InstructionException : System.Exception {
        IType iType;
        uint op;

        internal InstructionException(IType iType, uint op)
        {
            this.iType = iType;
            this.op = op;
        }

        internal string AddMethodName(string name)
        {
            string istr = " ";
            switch(iType) {
            case (IType.fieldOp):
                istr += (FieldOp)op;
                break;
            case (IType.methOp):
                istr += (MethodOp)op;
                break;
            case (IType.specialOp):
                istr += (SpecialOp)op;
                break;
            case (IType.typeOp):
                istr += (TypeOp)op;
                break;
            default:
                break;
            }
            return "NullPointer in instruction" + istr + " for method " + name;
        }

    }

    /// <summary>
    /// Error with descriptor types
    /// </summary>
    public class DescriptorException : System.Exception {

        public DescriptorException(string msg)
            :
          base("Descriptor for " + msg + " already exists")
        {
        }

        public DescriptorException()
            :
          base("Descriptor is a Def when a Ref is required")
        {
        }

    }

    /// <summary>
    /// Error for invalid PE file
    /// </summary>
    public class PEFileException : System.Exception {
        public PEFileException(string msg)
            : base("Error in PE File:  " + msg)
        {
        }
    }

    /**************************************************************************/

    // Various Enumerations for PEFiles
    /// <summary>
    /// flags for the assembly (.corflags)
    /// </summary>
    public enum CorFlags {
        CF_IL_ONLY=1,
        CF_32_BITREQUIRED=2,
        CF_STRONGNAMESIGNED=8,
        CF_TRACKDEBUGDATA=0x10000
    }

    /// <summary>
    /// subsystem for the assembly (.subsystem)
    /// </summary>
    public enum SubSystem {
        Native=1,
        Windows_GUI=2,
        Windows_CUI=3,
        OS2_CUI=5,
        POSIX_CUI=7,
        Native_Windows=8,
        Windows_CE_GUI=9
    }

    /// <summary>
    /// Hash algorithms for the assembly
    /// </summary>
    public enum HashAlgorithmType {
        None,
        SHA1=0x8004
    }

    /// <summary>
    /// Attributes for this assembly
    /// </summary>
    public enum AssemAttr {
        EnableJITCompileTracking=0x8000,
        DisableJITCompileOptimizer=0x4000
    }

    /// <summary>
    /// Method call conventions
    /// </summary>
    [FlagsAttribute]
    public enum CallConv {
        Default,
        Cdecl,
        Stdcall,
        Thiscall,
        Fastcall,
        Vararg,
        Generic=0x10,
        Instance=0x20,
        InstanceExplicit=0x60
    }

    /// <summary>
    /// Method Types for Events and Properties
    /// </summary>
    public enum MethodType {
        Setter=0x01,
        Getter,
        Other=0x04,
        AddOn=0x08,
        RemoveOn=0x10,
        Fire=0x20
    }

    /// <summary>
    /// Type custom modifier
    /// </summary>
    public enum CustomModifier {
        modreq=0x1F,
        modopt
    };

    /// <summary>
    /// Attibutes for a class
    /// </summary>
    [FlagsAttribute]
    public enum TypeAttr {
        Private,
        Public,
        NestedPublic,
        NestedPrivate,
        NestedFamily,
        NestedAssembly,
        NestedFamAndAssem,
        NestedFamOrAssem,
        SequentialLayout,
        ExplicitLayout=0x10,
        Interface=0x20,
        Abstract=0x80,
        PublicAbstract=0x81,
        Sealed=0x100,
        PublicSealed=0x101,
        SpecialName=0x400,
        RTSpecialName=0x800,
        Import=0x1000,
        Serializable=0x2000,
        UnicodeClass=0x10000,
        AutoClass=0x20000,
        BeforeFieldInit=0x100000
    }

    /// <summary>
    /// Attributes for a field
    /// </summary>
    [FlagsAttribute]
    public enum FieldAttr {
        Default,
        Private,
        FamAndAssem,
        Assembly,
        Family,
        FamOrAssem,
        Public,
        Static=0x10,
        PublicStatic=0x16,
        Initonly=0x20,
        Literal=0x40,
        Notserialized=0x80,
        SpecialName=0x200,
        RTSpecialName=0x400
    }

    /// <summary>
    /// Attributes for a method
    /// </summary>
    [FlagsAttribute]
    public enum MethAttr {
        Default,
        Private,
        FamAndAssem,
        Assembly,
        Family,
        FamOrAssem,
        Public,
        Static=0x0010,
        PublicStatic=0x16,
        Final=0x0020,
        PublicStaticFinal=0x36,
        Virtual=0x0040,
        PrivateVirtual,
        PublicVirtual=0x0046,
        HideBySig=0x0080,
        NewSlot=0x0100,
        Abstract=0x0400,
        SpecialName=0x0800,
        RTSpecialName=0x1000,
        SpecialRTSpecialName=0x1800,
        RequireSecObject=0x8000
    }

    /// <summary>
    /// Attributes for .pinvokeimpl method declarations
    /// </summary>
    [FlagsAttribute]
    public enum PInvokeAttr {
        ansi=2,
        unicode=4,
        autochar=6,
        lasterr=0x040,
        winapi=0x100,
        cdecl=0x200,
        stdcall=0x300,
        thiscall=0x400,
        fastcall=0x500
    }

    /// <summary>
    /// Implementation attributes for a method
    /// </summary>
    [FlagsAttribute]
    public enum ImplAttr {
        IL,
        Native,
        OPTIL,
        Runtime,
        Unmanaged,
        ForwardRef=0x10,
        PreserveSig=0x0080,
        InternalCall=0x1000,
        Synchronised=0x0020,
        Synchronized=0x0020,
        NoInLining=0x0008
    }

    /// <summary>
    /// Modes for a parameter
    /// </summary>
    [FlagsAttribute]
    public enum ParamAttr {
        Default,
        In,
        Out,
        Opt=4
    }

    /// <summary>
    /// Flags for a generic parameter
    /// </summary>
    public enum GenericParamAttr {
        NonVariant,
        Covariant,
        Contravariant,
        ReferenceType=0x4,
        RequireDefaultCtor=0x10
    }

    /// <summary>
    /// Which version of PE file to build
    /// </summary>
    public enum NetVersion {
        Everett,   /* version 1.1.4322  */
        Whidbey40, /* version 2.0.40607 */
        Whidbey41  /* version 2.0.41202 */
    }

    /// <summary>
    /// CIL instructions
    /// </summary>
    public enum Op {
        nop,
        breakOp,
        ldarg_0,
        ldarg_1,
        ldarg_2,
        ldarg_3,
        ldloc_0,
        ldloc_1,
        ldloc_2,
        ldloc_3,
        stloc_0,
        stloc_1,
        stloc_2,
        stloc_3,
        ldnull=0x14,
        ldc_i4_m1,
        ldc_i4_0,
        ldc_i4_1,
        ldc_i4_2,
        ldc_i4_3,
        ldc_i4_4,
        ldc_i4_5,
        ldc_i4_6,
        ldc_i4_7,
        ldc_i4_8,
        dup=0x25,
        pop,
        ret=0x2A,
        ldind_i1=0x46,
        ldind_u1,
        ldind_i2,
        ldind_u2,
        ldind_i4,
        ldind_u4,
        ldind_i8,
        ldind_i,
        ldind_r4,
        ldind_r8,
        ldind_ref,
        stind_ref,
        stind_i1,
        stind_i2,
        stind_i4,
        stind_i8,
        stind_r4,
        stind_r8,
        add,
        sub,
        mul,
        div,
        div_un,
        rem,
        rem_un,
        and,
        or,
        xor,
        shl,
        shr,
        shr_un,
        neg,
        not,
        conv_i1,
        conv_i2,
        conv_i4,
        conv_i8,
        conv_r4,
        conv_r8,
        conv_u4,
        conv_u8,
        conv_r_un=0x76,
        throwOp=0x7A,
        conv_ovf_i1_un=0x82,
        conv_ovf_i2_un,
        conv_ovf_i4_un,
        conv_ovf_i8_un,
        conf_ovf_u1_un,
        conv_ovf_u2_un,
        conv_ovf_u4_un,
        conv_ovf_u8_un,
        conv_ovf_i_un,
        conv_ovf_u_un,
        ldlen=0x8E,
        ldelem_i1=0x90,
        ldelem_u1,
        ldelem_i2,
        ldelem_u2,
        ldelem_i4,
        ldelem_u4,
        ldelem_i8,
        ldelem_i,
        ldelem_r4,
        ldelem_r8,
        ldelem_ref,
        stelem_i,
        stelem_i1,
        stelem_i2,
        stelem_i4,
        stelem_i8,
        stelem_r4,
        stelem_r8,
        stelem_ref,
        conv_ovf_i1=0xb3,
        conv_ovf_u1,
        conv_ovf_i2,
        conv_ovf_u2,
        conv_ovf_i4,
        conv_ovf_u4,
        conv_ovf_i8,
        conv_ovf_u8,
        ckfinite=0xC3,
        conv_u2=0xD1,
        conv_u1,
        conv_i,
        conv_ovf_i,
        conv_ovf_u,
        add_ovf,
        add_ovf_un,
        mul_ovf,
        mul_ovf_un,
        sub_ovf,
        sub_ovf_un,
        endfinally,
        stind_i=0xDF,
        conv_u,
        arglist=0xFE00,
        ceq,
        cgt,
        cgt_un,
        clt,
        clt_un,
        localloc=0xFE0F,
        endfilter=0xFE11,
        volatile_=0xFE13,
        tail_,
        cpblk=0xFE17,
        initblk,
        rethrow=0xFE1A,
        refanytype=0xFE1D,
        readOnly
    }

    /// <summary>
    /// CIL instructions requiring an integer parameter
    /// </summary>
    public enum IntOp {
        ldarg_s=0x0E,
        ldarga_s,
        starg_s,
        ldloc_s,
        ldloca_s,
        stloc_s,
        ldc_i4_s=0x1F,
        ldc_i4,
        ldarg=0xFE09,
        ldarga,
        starg,
        ldloc,
        ldloca,
        stloc,
        unaligned=0xFE12
    }

    /// <summary>
    /// CIL instructions requiring a field parameter
    /// </summary>
    public enum FieldOp {
        ldfld=0x7B,
        ldflda,
        stfld,
        ldsfld,
        ldsflda,
        stsfld,
        ldtoken=0xD0
    }

    /// <summary>
    /// CIL instructions requiring a method parameter
    /// </summary>
    public enum MethodOp {
        jmp=0x27,
        call,
        callvirt=0x6F,
        newobj=0x73,
        ldtoken=0xD0,
        ldftn=0xFE06,
        ldvirtfn
    }

    /// <summary>
    /// CIL instructions requiring a type parameter
    /// </summary>
    public enum TypeOp {
        cpobj=0x70,
        ldobj,
        castclass=0x74,
        isinst,
        unbox=0x79,
        stobj=0x81,
        box=0x8C,
        newarr,
        ldelema=0x8F,
        ldelem_any=0xA3,
        stelem_any,
        unbox_any,
        refanyval=0xC2,
        mkrefany=0xC6,
        ldtoken=0xD0,
        initobj=0xFE15,
        constrained,
        sizeOf=0xFE1C
    }

    /// <summary>
    /// CIL branch instructions
    /// </summary>
    public enum BranchOp {
        br_s=0x2B,
        brfalse_s,
        brtrue_s,
        beq_s,
        bge_s,
        bgt_s,
        ble_s,
        blt_s,
        bne_un_s,
        bge_un_s,
        bgt_un_s,
        ble_un_s,
        blt_un_s,
        br,
        brfalse,
        brtrue,
        beq,
        bge,
        bgt,
        ble,
        blt,
        bne_un,
        bge_un,
        bgt_un,
        ble_un,
        blt_un,
        leave=0xDD,
        leave_s
    }

    public enum SpecialOp {
        ldc_i8=0x21,
        ldc_r4,
        ldc_r8,
        calli=0x29,
        Switch=0x45,
        ldstr=0x72
    }

    /// <summary>
    /// Index for all the tables in the meta data
    /// </summary>
    public enum MDTable {
        Module,
        TypeRef,
        TypeDef,
        Field=0x04,
        Method=0x06,
        Param=0x08,
        InterfaceImpl,
        MemberRef,
        Constant,
        CustomAttribute,
        FieldMarshal,
        DeclSecurity,
        ClassLayout,
        FieldLayout,
        StandAloneSig,
        EventMap,
        Event=0x14,
        PropertyMap,
        Property=0x17,
        MethodSemantics,
        MethodImpl,
        ModuleRef,
        TypeSpec,
        ImplMap,
        FieldRVA,
        Assembly=0x20,
        AssemblyProcessor,
        AssemblyOS,
        AssemblyRef,
        AssemblyRefProcessor,
        AssemblyRefOS,
        File,
        ExportedType,
        ManifestResource,
        NestedClass,
        GenericParam,
        MethodSpec,
        GenericParamConstraint,
        MaxMDTable
    }

    internal enum NativeTypeIx {
        Void=0x01,
        Boolean,
        I1,
        U1,
        I2,
        U2,
        I4,
        U4,
        I8,
        U8,
        R4,
        R8,
        SysChar,
        Variant,
        Currency,
        Ptr,
        Decimal,
        Date,
        BStr,
        LPStr,
        LPWStr,
        LPTStr,
        FixedSysString,
        ObjectRef,
        IUnknown,
        IDispatch,
        Struct,
        Intf,
        SafeArray,
        FixedArray,
        Int,
        UInt,
        NestedStruct,
        ByValStr,
        AnsiBStr,
        TBStr,
        VariantBool,
        Func,
        AsAny=0x28,
        Array=0x2A,
        LPStruct,
        CustomMarshaller,
        Error
    }

    public enum SafeArrayType {
        int16=2,
        int32,
        float32,
        float64,
        currency,
        date,
        bstr,
        dispatch,
        error,
        boolean,
        variant,
        unknown,
        Decimal,
        int8=16,
        uint8,
        uint16,
        uint32,
        Int=22,
        UInt
    }

    internal enum CIx {
        TypeDefOrRef,
        HasConstant,
        HasCustomAttr,
        HasFieldMarshal,
        HasDeclSecurity,
        MemberRefParent,
        HasSemantics,
        MethodDefOrRef,
        MemberForwarded,
        Implementation,
        CustomAttributeType,
        ResolutionScope,
        TypeOrMethodDef,
        MaxCIx
    }

    internal enum MapType {
        eventMap,
        propertyMap,
        nestedClass
    }

    public enum ElementType : byte {
        End,
        Void,
        Boolean,
        Char,
        I1,
        U1,
        I2,
        U2,
        I4,
        U4,
        I8,
        U8,
        R4,
        R8,
        String,
        Ptr,
        ByRef,
        ValueType,
        Class,
        Var,
        Array,
        GenericInst,
        TypedByRef,
        I=0x18,
        U,
        FnPtr=0x1B,
        Object,
        SZArray,
        MVar,
        CmodReqd,
        CmodOpt,
        Internal,
        Modifier=0x40,
        Sentinel,
        Pinned=0x45,
        ClassType=0x50
    }

    public enum SecurityAction {
        Request=0x01,
        Demand,
        Assert,
        Deny,
        PermitOnly,
        LinkDemand,
        InheritanceDemand,
        RequestMinimum,
        RequestOptional,
        RequestRefuse,
        PreJITGrant,
        PreJITDeny,
        NonCASDemand,
        NonCASLinkDemand,
        NonCASInheritanceDemand
    }

    internal enum IType {
        op,
        methOp,
        fieldOp,
        typeOp,
        specialOp,
        int8Op,
        uint8Op,
        uint16Op,
        int32Op,
        branchOp
    }

    /**************************************************************************/
    /// <summary>
    /// Abstract class to represent a row of the Meta Data Tables
    /// </summary>
    public abstract class TableRow {
        internal PEReader buffer;
        private uint row = 0;
        /// <summary>
        /// The index of the Meta Data Table containing this element
        /// </summary>
        protected MDTable tabIx;

        /*-------------------- Constructors ---------------------------------*/

        internal TableRow()
        {
        }

        internal TableRow(PEReader buff, uint ix, MDTable tableIx)
        {
            buffer = buff;
            row = ix;
            tabIx = tableIx;
        }

        /// <summary>
        /// The row number of this element in the Meta Data Table
        /// </summary>
        public uint Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }
    }

    /****************************************************/
    /// <summary>
    /// Base class for all Meta Data table elements
    /// </summary>
    public abstract class MetaDataElement : TableRow, IComparable {
        /// <summary>
        /// The list of custom attributes associated with this meta data element
        /// </summary>
        protected ArrayList customAttributes;
        protected bool done = false;
        protected bool sortTable = false;
        internal bool unresolved = false;

        /*-------------------- Constructors ---------------------------------*/

        internal MetaDataElement()
        {
        }

        /// <summary>
        /// Get any custom attributes associated with this meta data element
        /// </summary>
        /// <returns>Array of custom attribute descriptors</returns>
        public CustomAttribute[] GetCustomAttributes()
        {
            if(customAttributes == null)
                return new CustomAttribute[0];
            return (CustomAttribute[])customAttributes.ToArray(typeof(CustomAttribute));
        }

        /// <summary>
        /// Associate some custom attribute(s) with this meta data element
        /// </summary>
        /// <param name="cas">list of custom attributes</param>
        public void SetCustomAttributes(CustomAttribute[] cas)
        {
            if(cas == null)
                customAttributes = null;
            else
                customAttributes = new ArrayList(cas);
        }

        internal virtual bool isDef()
        {
            return false;
        }

        internal virtual void Resolve(PEReader buff)
        {
        }

        internal virtual void ResolveDetails(PEReader buff)
        {
        }

        internal virtual uint GetCodedIx(CIx code)
        {
            return 0;
        }

        internal bool NeedToSort()
        {
            return sortTable;
        }

        internal virtual uint SortKey()
        {
            throw new PEFileException("Trying to sort table of " + this);
            //return 0; 
        }

        /// <summary>
        /// Add a custom attribute to this item
        /// </summary>
        /// <param name="ctorMeth">the constructor method for this attribute</param>
        /// <param name="val">the byte value of the parameters</param>
        public void AddCustomAttribute(Method ctorMeth, byte[] val)
        {
            if(customAttributes == null) {
                customAttributes = new ArrayList();
            }
            customAttributes.Add(new CustomAttribute(this, ctorMeth, val));
        }

        /// <summary>
        /// Add a custom attribute to this item
        /// </summary>
        /// <param name="ctorMeth">the constructor method for this attribute</param>
        /// <param name="cVals">the constant values of the parameters</param>
        public void AddCustomAttribute(Method ctorMeth, Constant[] cVals)
        {
            if(customAttributes == null) {
                customAttributes = new ArrayList();
            }
            customAttributes.Add(new CustomAttribute(this, ctorMeth, cVals));
        }

        /// <summary>
        /// Associate a custom attribute with this meta data element
        /// </summary>
        public void AddCustomAttribute(CustomAttribute ca)
        {
            if(customAttributes == null) {
                customAttributes = new ArrayList();
            }
            customAttributes.Add(ca);
        }

        internal uint Token()
        {
            if(Row == 0)
                throw new Exception("Meta data token is zero!!");
            return (((uint)tabIx << 24) | Row);
        }

        internal void BuildMDTables(MetaDataOut md)
        {
            if(done)
                return;
            done = true;
            if(Diag.DiagOn)
                Console.WriteLine("In BuildMDTables");
            BuildTables(md);
            if(customAttributes != null) {
                for(int i=0; i < customAttributes.Count; i++) {
                    CustomAttribute ca = (CustomAttribute)customAttributes[i];
                    ca.BuildTables(md);
                }
            }
        }

        internal virtual void BuildTables(MetaDataOut md)
        {
        }

        internal virtual void BuildSignatures(MetaDataOut md)
        {
            done = false;
        }

        internal virtual void AddToTable(MetaDataOut md)
        {
            md.AddToTable(tabIx, this);
        }

        internal virtual void Write(PEWriter output)
        {
        }

        internal virtual string NameString()
        {
            return "NoName";
        }

        internal void DescriptorError(MetaDataElement elem)
        {
            throw new DescriptorException(elem.NameString());
        }
        #region IComparable Members

        public int CompareTo(object obj)
        {
            uint otherKey = ((MetaDataElement)obj).SortKey();
            uint thisKey = SortKey();
            if(thisKey == otherKey) {
                if(this is GenericParam) {
                    if(((GenericParam)this).Index < ((GenericParam)obj).Index)
                        return -1;
                    else
                        return 1;
                }
                return 0;
            }
            if(thisKey < otherKey)
                return -1;
            return 1;
        }

        #endregion
    }
    /**************************************************************************/
    /// <summary>
    /// Layout information for a class (.class [sequential | explicit])
    /// </summary>
    internal class ClassLayout : MetaDataElement {
        ClassDef parent;
        ushort packSize = 0;
        uint classSize = 0;
        uint parentIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal ClassLayout(int pack, int cSize, ClassDef par)
        {
            packSize = (ushort)pack;
            classSize = (uint)cSize;
            parent = par;
            tabIx = MDTable.ClassLayout;
        }

        internal ClassLayout(ushort pack, uint cSize, ClassDef par)
        {
            packSize = pack;
            classSize = cSize;
            parent = par;
            tabIx = MDTable.ClassLayout;
        }

        internal ClassLayout(PEReader buff)
        {
            packSize = buff.ReadUInt16();
            classSize = buff.ReadUInt32();
            parentIx = buff.GetIndex(MDTable.TypeDef);
            tabIx = MDTable.ClassLayout;
        }

        internal static ClassLayout FindLayout(PEReader buff, ClassDef paren, uint classIx)
        {
            buff.SetElementPosition(MDTable.ClassLayout, 0);
            for(int i=0; i < buff.GetTableSize(MDTable.ClassLayout); i++) {
                ushort packSize = buff.ReadUInt16();
                uint classSize = buff.ReadUInt32();
                if(buff.GetIndex(MDTable.TypeDef) == classIx)
                    return new ClassLayout(packSize, classSize, paren);
            }
            return null;
        }

        internal static void Read(PEReader buff, TableRow[] layouts)
        {
            for(int i=0; i < layouts.Length; i++) {
                layouts[i] = new ClassLayout(buff);
            }
        }

        internal override void Resolve(PEReader buff)
        {
            parent = (ClassDef)buff.GetElement(MDTable.TypeDef, parentIx);
            if(parent != null)
                parent.Layout = this;
        }

        /*------------------------- public set and get methods --------------------------*/

        public void SetPack(int pack)
        {
            packSize = (ushort)pack;
        }
        public int GetPack()
        {
            return (int)packSize;
        }
        public void SetSize(int size)
        {
            classSize = (uint)size;
        }
        public int GetSize()
        {
            return (int)classSize;
        }

        /*----------------------------- internal functions ------------------------------*/

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(tabIx, this);
        }

        internal static uint Size(MetaData md)
        {
            return 6 + md.TableIndexSize(MDTable.TypeDef);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(packSize);
            output.Write(classSize);
            output.WriteIndex(MDTable.TypeDef, parent.Row);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Summary description for ConstantElem.
    /// </summary>
    internal class ConstantElem : MetaDataElement {
        MetaDataElement parent;
        Constant cValue;
        uint valIx = 0, parentIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal ConstantElem(MetaDataElement parent, Constant val)
        {
            this.parent = parent;
            cValue = val;
            sortTable = true;
            tabIx = MDTable.Constant;
        }

        internal ConstantElem(PEReader buff)
        {
            byte constType = buff.ReadByte();
            byte pad = buff.ReadByte();
            parentIx = buff.GetCodedIndex(CIx.HasConstant);
            //valIx = buff.GetBlobIx();
            cValue = buff.GetBlobConst(constType);
            sortTable = true;
            tabIx = MDTable.Constant;
        }

        internal override void Resolve(PEReader buff)
        {
            parent = buff.GetCodedElement(CIx.HasConstant, parentIx);
            if(parent != null) {
                if(parent is Param)
                    ((Param)parent).AddDefaultValue(cValue);
                else if(parent is FieldDef)
                    ((FieldDef)parent).AddValue(cValue);
                else
                    ((Property)parent).AddInitValue(cValue);
            }
        }

        internal static void Read(PEReader buff, TableRow[] consts)
        {
            for(int i=0; i < consts.Length; i++)
                consts[i] = new ConstantElem(buff);
        }

        /*----------------------------- internal functions ------------------------------*/

        internal override uint SortKey()
        {
            return (parent.Row << MetaData.CIxShiftMap[(uint)CIx.HasConstant]) 
        | parent.GetCodedIx(CIx.HasConstant);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Constant, this);
            valIx = cValue.GetBlobIndex(md);
        }

        internal static uint Size(MetaData md)
        {
            return 2 + md.CodedIndexSize(CIx.HasConstant) + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(cValue.GetTypeIndex());
            output.Write((byte)0);
            output.WriteCodedIndex(CIx.HasConstant, parent);
            output.BlobIndex(valIx);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a Custom Attribute (.custom) 
    /// </summary>
    public class CustomAttribute : MetaDataElement {
        internal static readonly ushort prolog = 0x0001;
        private static readonly int initSize = 5;
        MetaDataElement parent;
        Method type;
        uint valIx, parentIx, typeIx;
        Constant[] argVals, vals;
        byte[] byteVal;
        ushort numNamed = 0;
        string[] names;
        bool[] isField;
        bool changed = false;

        /*-------------------- Constructors ---------------------------------*/

        internal CustomAttribute(MetaDataElement paren, Method constrType,
          Constant[] val)
        {
            parent = paren;
            type = constrType;
            argVals = val;
            changed = true;
            sortTable = true;
            tabIx = MDTable.CustomAttribute;
        }

        internal CustomAttribute(MetaDataElement paren, Method constrType,
          byte[] val)
        {
            parent = paren;
            type = constrType;
            tabIx = MDTable.CustomAttribute;
            byteVal = val;
            sortTable = true;
            changed = true;
        }

        internal CustomAttribute(PEReader buff)
        {
            parentIx = buff.GetCodedIndex(CIx.HasCustomAttr);
            typeIx = buff.GetCodedIndex(CIx.CustomAttributeType);
            valIx = buff.GetBlobIx();
            sortTable = true;
            tabIx = MDTable.CustomAttribute;
        }

        internal static void Read(PEReader buff, TableRow[] attrs)
        {
            for(int i=0; i < attrs.Length; i++) {
                attrs[i] = new CustomAttribute(buff);
            }
        }

        internal override void Resolve(PEReader buff)
        {
            parent = buff.GetCodedElement(CIx.HasCustomAttr, parentIx);
            if(parent == null)
                return;
            parent.AddCustomAttribute(this);
            type = (Method)buff.GetCodedElement(CIx.CustomAttributeType, typeIx);
            byteVal = buff.GetBlob(valIx);
        }

        /*------------------------- public set and get methods --------------------------*/

        public void AddFieldOrProp(string name, Constant val, bool isFld)
        {
            if((byteVal != null) && !changed)
                DecodeCustomAttributeBlob();
            if(numNamed == 0) {
                names = new string[initSize];
                vals = new Constant[initSize];
                isField = new bool[initSize];
            } else if(numNamed >= names.Length) {
                string[] tmpNames = names;
                Constant[] tmpVals = vals;
                bool[] tmpField = isField;
                names = new String[names.Length + initSize];
                vals = new Constant[vals.Length + initSize];
                isField = new bool[isField.Length + initSize];
                for(int i = 0; i < numNamed; i++) {
                    names[i] = tmpNames[i];
                    vals[i] = tmpVals[i];
                    isField[i] = tmpField[i];
                }
            }
            names[numNamed] = name;
            vals[numNamed] = val;
            isField[numNamed++] = isFld;
            changed = true;
        }

        public Constant[] Args
        {
            get
            {
                if(!changed && (byteVal != null))
                    DecodeCustomAttributeBlob();
                return argVals;
            }
            set
            {
                argVals = value;
                changed = true;
            }
        }

        public string[] GetNames()
        {
            return names;
        }

        public bool[] GetIsField()
        {
            return isField;
        }
        public Constant[] GetNamedArgs()
        {
            return vals;
        }

        /*----------------------------- internal functions ------------------------------*/

        internal void DecodeCustomAttributeBlob()
        {
            MemoryStream caBlob = new MemoryStream(byteVal);
            BinaryReader blob = new BinaryReader(caBlob, System.Text.Encoding.UTF8);
            if(blob.ReadUInt16() != CustomAttribute.prolog)
                throw new PEFileException("Invalid Custom Attribute Blob");
            Type[] parTypes = type.GetParTypes();
            argVals = new Constant[parTypes.Length];
            for(int i=0; i < parTypes.Length; i++) {
                Type argType = parTypes[i];
                bool arrayConst = argType is Array;
                if(arrayConst)
                    argType = ((ZeroBasedArray)(parTypes[i])).ElemType();
                bool boxed = argType is SystemClass;
                int eType = argType.GetTypeIndex();
                if(arrayConst) {
                    Constant[] elems = new Constant[blob.ReadUInt32()];
                    for(int j=0; j < elems.Length; j++) {
                        if(boxed) {
                            eType = blob.ReadByte();
                            elems[j] = new BoxedSimpleConst((SimpleConstant)PEReader.ReadConst(eType, blob));
                        } else {
                            elems[j] = PEReader.ReadConst(eType, blob);
                        }
                    }
                    argVals[i] = new ArrayConst(elems);
                } else if(boxed) {
                    argVals[i] = new BoxedSimpleConst((SimpleConstant)PEReader.ReadConst(blob.ReadByte(), blob));
                } else {
                    argVals[i] = PEReader.ReadConst(eType, blob);
                }
            }
            uint numNamed = 0;
            if(blob.BaseStream.Position < byteVal.Length)
                numNamed = blob.ReadUInt16();
            if(numNamed > 0) {
                names = new string[numNamed];
                vals = new Constant[numNamed];
                isField = new bool[numNamed];
                for(int i=0; i < numNamed; i++) {
                    isField[i] = blob.ReadByte() == 0x53;
                    int eType = blob.ReadByte();
                    names[i] = blob.ReadString();
                    vals[i] = PEReader.ReadConst(eType, blob);
                }
            }
        }

        internal void AddFieldOrProps(string[] names, Constant[] vals, bool[] isField)
        {
            this.names = names;
            this.vals = vals;
            this.isField = isField;
            numNamed = (ushort)names.Length;
        }

        internal void SetBytes(byte[] bytes)
        {
            this.byteVal = bytes;
        }

        internal Method GetCAType()
        {
            return type;
        }

        internal override uint SortKey()
        {
            return (parent.Row << MetaData.CIxShiftMap[(uint)CIx.HasCustomAttr]) 
        | parent.GetCodedIx(CIx.HasCustomAttr);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(tabIx, this);
            type.BuildMDTables(md);
            // more adding to tables if data is not bytes
            if(changed || (byteVal == null)) {
                MemoryStream str = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(str);
                bw.Write((ushort)1);
                if(argVals != null) {
                    for(int i=0; i < argVals.Length; i++) {
                        argVals[i].Write(bw);
                    }
                }
                bw.Write(numNamed);
                for(int i=0; i < numNamed; i++) {
                    if(isField[i])
                        bw.Write(Field.FieldTag);
                    else
                        bw.Write(Property.PropertyTag);
                    bw.Write(vals[i].GetTypeIndex());
                    bw.Write(names[i]);  // check this is the right format!!!
                    vals[i].Write(bw);
                }
                byteVal = str.ToArray();
            }
            valIx = md.AddToBlobHeap(byteVal);
        }

        internal static uint Size(MetaData md)
        {
            return md.CodedIndexSize(CIx.HasCustomAttr) + md.CodedIndexSize(CIx.CustomAttributeType) + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteCodedIndex(CIx.HasCustomAttr, parent);
            output.WriteCodedIndex(CIx.CustomAttributeType, type);
            output.BlobIndex(valIx);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for security permissions for a class or a method NOT YET IMPLEMENTED
    /// </summary>
    public class DeclSecurity : MetaDataElement {
        SecurityAction action;
        MetaDataElement parent;
        uint parentIx = 0, permissionIx;
        byte[] permissionSet;

        /*-------------------- Constructors ---------------------------------*/

        internal DeclSecurity(MetaDataElement paren, SecurityAction act, byte[] perSet)
        {
            parent = paren;
            action = act;
            permissionSet = perSet;
            sortTable = true;
            tabIx = MDTable.DeclSecurity;
        }

        internal DeclSecurity(PEReader buff)
        {
            action = (SecurityAction)buff.ReadUInt16();
            parentIx = buff.GetCodedIndex(CIx.HasDeclSecurity);
            permissionSet = buff.GetBlob();
            sortTable = true;
            tabIx = MDTable.DeclSecurity;
        }

        internal static void Read(PEReader buff, TableRow[] secs)
        {
            for(int i=0; i < secs.Length; i++)
                secs[i] = new DeclSecurity(buff);
        }

        internal static DeclSecurity FindSecurity(PEReader buff, MetaDataElement paren, uint codedParIx)
        {
            buff.SetElementPosition(MDTable.DeclSecurity, 0);
            for(int i=0; i < buff.GetTableSize(MDTable.DeclSecurity); i++) {
                uint act = buff.ReadUInt16();
                if(buff.GetCodedIndex(CIx.HasDeclSecurity) == codedParIx)
                    return new DeclSecurity(paren, (SecurityAction)act, buff.GetBlob());
                uint junk = buff.GetBlobIx();
            }
            return null;
        }

        internal override void Resolve(PEReader buff)
        {
            parent = buff.GetCodedElement(CIx.HasDeclSecurity, parentIx);
            if(parent != null) {
                if(parent is ClassDef)
                    ((ClassDef)parent).AddSecurity(this);
                if(parent is Assembly)
                    ((Assembly)parent).AddSecurity(this);
                if(parent is MethodDef)
                    ((MethodDef)parent).AddSecurity(this);
            }
        }

        internal override uint SortKey()
        {
            return (parent.Row << MetaData.CIxShiftMap[(uint)CIx.HasDeclSecurity]) 
        | parent.GetCodedIx(CIx.HasDeclSecurity);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.DeclSecurity, this);
            permissionIx = md.AddToBlobHeap(permissionSet);
        }

        internal static uint Size(MetaData md)
        {
            return 2 + md.CodedIndexSize(CIx.HasDeclSecurity) + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write((UInt16)action);   // or should this be 2 bytes??
            output.WriteCodedIndex(CIx.HasDeclSecurity, parent);
            output.BlobIndex(permissionIx);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a class defined in another module of THIS assembly 
    /// and exported (.class extern)
    /// </summary>
    internal class ExternClass : MetaDataElement {
        MetaDataElement implementation;
        uint flags, typeDefId = 0;
        uint implIx = 0, nameIx = 0, nameSpaceIx = 0;
        string nameSpace, name;

        /*-------------------- Constructors ---------------------------------*/

        internal ExternClass(TypeAttr attr, string ns, string name, MetaDataElement paren)
        {
            flags = (uint)attr;
            nameSpace = ns;
            this.name = name;
            implementation = paren;
            tabIx = MDTable.ExportedType;
        }

        public ExternClass(PEReader buff)
        {
            flags = buff.ReadUInt32();
            typeDefId = buff.ReadUInt32();
            name = buff.GetString();
            nameSpace = buff.GetString();
            implIx = buff.GetCodedIndex(CIx.Implementation);
            tabIx = MDTable.ExportedType;
        }

        internal static void Read(PEReader buff, TableRow[] eClasses)
        {
            for(int i=0; i < eClasses.Length; i++)
                eClasses[i] = new ExternClass(buff);
        }

        internal static void GetClassRefs(PEReader buff, TableRow[] eClasses)
        {
            for(uint i=0; i < eClasses.Length; i++) {
                uint junk = buff.ReadUInt32();
                junk = buff.ReadUInt32();
                string name = buff.GetString();
                string nameSpace = buff.GetString();
                uint implIx = buff.GetCodedIndex(CIx.Implementation);
                eClasses[i] = new ClassRef(implIx, nameSpace, name);
                eClasses[i].Row = i+1;
            }
        }

        internal override void Resolve(PEReader buff)
        {
            implementation = buff.GetCodedElement(CIx.Implementation, implIx);
            while(implementation is ExternClass)
                implementation = ((ExternClass)implementation).implementation;
            ((ModuleFile)implementation).fileModule.AddExternClass(this);
        }

        internal string NameSpace()
        {
            return nameSpace;
        }
        internal string Name()
        {
            return name;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.ExportedType, this);
            nameSpaceIx = md.AddToStringsHeap(nameSpace);
            nameIx = md.AddToStringsHeap(name);
            if(implementation is ModuleRef) {
                ModuleFile mFile = ((ModuleRef)implementation).modFile;
                mFile.BuildMDTables(md);
                implementation = mFile;
            }
        }

        internal static uint Size(MetaData md)
        {
            return 8 + 2* md.StringsIndexSize() + md.CodedIndexSize(CIx.Implementation);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(flags);
            output.Write(0);
            output.StringsIndex(nameIx);
            output.StringsIndex(nameSpaceIx);
            output.WriteCodedIndex(CIx.Implementation, implementation);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 17;
            case (CIx.Implementation):
                return 2;
            }
            return 0;
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Base class for Event and Property descriptors
    /// </summary>
    public abstract class Feature : MetaDataElement {
        private static readonly int INITSIZE = 5;
        private static readonly ushort specialName = 0x200;
        private static readonly ushort rtsSpecialName = 0x400;
        private static readonly ushort noSpecialName = 0xFDFF;
        private static readonly ushort noRTSSpecialName = 0xFBFF;

        protected ClassDef parent;
        protected ushort flags = 0;
        protected string name;
        protected int tide = 0;
        protected uint nameIx;
        protected MethodSemantics[] methods = new MethodSemantics[INITSIZE];

        /*-------------------- Constructors ---------------------------------*/

        internal Feature(string name, ClassDef par)
        {
            parent = par;
            this.name = name;
        }

        internal Feature()
        {
        }

        internal static string[] GetFeatureNames(PEReader buff, MDTable tabIx, MDTable mapTabIx,
          ClassDef theClass, uint classIx)
        {
            buff.SetElementPosition(mapTabIx, 0);
            uint start = 0, end = 0, i = 0;
            for(; (i < buff.GetTableSize(tabIx)) && (start == 0); i++) {
                if(buff.GetIndex(MDTable.TypeDef) == classIx) {
                    start = buff.GetIndex(tabIx);
                }
            }
            if(start == 0)
                return null;
            if(i < buff.GetTableSize(mapTabIx)) {
                uint junk = buff.GetIndex(MDTable.TypeDef);
                end = buff.GetIndex(tabIx);
            } else
                end = buff.GetTableSize(tabIx);
            if(tabIx == MDTable.Event)
                theClass.eventIx = start;
            else
                theClass.propIx = start;
            string[] names = new string[end-start];
            buff.SetElementPosition(tabIx, start);
            for(i=start; i < end; i++) {
                uint junk = buff.ReadUInt16();
                names[i] = buff.GetString();
                if(tabIx == MDTable.Event)
                    junk = buff.GetCodedIndex(CIx.TypeDefOrRef);
                else
                    junk = buff.GetBlobIx();
            }
            return names;
        }


        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Set the specialName attribute for this Event or Property
        /// </summary>
        public void SetSpecialName()
        {
            flags |= specialName;
        }
        public bool HasSpecialName()
        {
            return (flags & specialName) != 0;
        }
        public void ClearSpecialName()
        {
            flags &= noSpecialName;
        }

        /// <summary>
        /// Set the RTSpecialName attribute for this Event or Property
        /// </summary>
        public void SetRTSpecialName()
        {
            flags |= rtsSpecialName;
        }
        public bool HasRTSSpecialName()
        {
            return (flags & rtsSpecialName) != 0;
        }
        public void ClearRTSSpecialName()
        {
            flags &= noRTSSpecialName;
        }

        public string Name()
        {
            return name;
        }
        public void SetName(string nam)
        {
            name = nam;
        }

        internal void AddMethod(MethodSemantics meth)
        {
            if(tide == methods.Length) {
                MethodSemantics[] mTmp = methods;
                methods = new MethodSemantics[tide * 2];
                for(int i=0; i < tide; i++) {
                    methods[i] = mTmp[i];
                }
            }
            methods[tide++] = meth;
        }

        public void AddMethod(MethodDef meth, MethodType mType)
        {
            AddMethod(new MethodSemantics(mType, meth, this));
        }

        public MethodDef GetMethod(MethodType mType)
        {
            for(int i=0; i < tide; i++) {
                if(methods[i].GetMethodType() == mType)
                    return methods[i].GetMethod();
            }
            return null;
        }

        public void RemoveMethod(MethodDef meth)
        {
            bool found = false;
            for(int i=0; i < tide; i++) {
                if(found)
                    methods[i-1] = methods[i];
                else if(methods[i].GetMethod() == meth)
                    found = true;
            }
        }

        public void RemoveMethod(MethodType mType)
        {
            bool found = false;
            for(int i=0; i < tide; i++) {
                if(found)
                    methods[i-1] = methods[i];
                else if(methods[i].GetMethodType() == mType)
                    found = true;
            }
        }

        internal void SetParent(ClassDef paren)
        {
            parent = paren;
        }

        internal ClassDef GetParent()
        {
            return parent;
        }


    }
    /*****************************************************************************/
    /// <summary>
    /// Descriptor for an event
    /// </summary>
    public class Event : Feature {
        Type eventType;
        uint typeIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal Event(string name, Type eType, ClassDef parent)
            : base(name, parent)
        {
            eventType = eType;
            tabIx = MDTable.Event;
        }

        internal Event(PEReader buff)
        {
            flags = buff.ReadUInt16();
            name = buff.GetString();
            typeIx = buff.GetCodedIndex(CIx.TypeDefOrRef);
            tabIx = MDTable.Event;
        }

        internal static void Read(PEReader buff, TableRow[] events)
        {
            for(int i=0; i < events.Length; i++)
                events[i] = new Event(buff);
        }

        internal static string[] ReadNames(PEReader buff, ClassDef theClass, uint classIx)
        {
            return Feature.GetFeatureNames(buff, MDTable.Event, MDTable.EventMap, theClass, classIx);
        }

        internal override void Resolve(PEReader buff)
        {
            eventType = (Type)buff.GetCodedElement(CIx.TypeDefOrRef, typeIx);
        }

        /*------------------------- public set and get methods --------------------------*/

        public Type GetEventType()
        {
            return eventType;
        }

        /*----------------------------- internal functions ------------------------------*/

        internal void ChangeRefsToDefs(ClassDef newType, ClassDef[] oldTypes)
        {
            throw new NotYetImplementedException("Merge for Events");
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Event, this);
            nameIx = md.AddToStringsHeap(name);
            eventType.BuildMDTables(md);
            for(int i=0; i < tide; i++) {
                methods[i].BuildMDTables(md);
            }
        }

        internal static uint Size(MetaData md)
        {
            return 2 + md.StringsIndexSize() + md.CodedIndexSize(CIx.TypeDefOrRef);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(flags);
            output.StringsIndex(nameIx);
            output.WriteCodedIndex(CIx.TypeDefOrRef, eventType);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 10;
            case (CIx.HasSemantics):
                return 0;
            }
            return 0;
        }
    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for the Property of a class
    /// </summary>
    public class Property : Feature {
        internal static readonly byte PropertyTag = 0x8;
        Constant constVal;
        uint typeBlobIx = 0;
        Type[] parList;
        Type returnType;
        uint numPars = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal Property(string name, Type retType, Type[] pars, ClassDef parent)
            : base(name, parent)
        {
            returnType = retType;
            parList = pars;
            if(pars != null)
                numPars = (uint)pars.Length;
            tabIx = MDTable.Property;
        }

        internal Property(PEReader buff)
        {
            flags = buff.ReadUInt16();
            name = buff.GetString();
            typeBlobIx = buff.GetBlobIx();
            tabIx = MDTable.Property;
        }

        internal static void Read(PEReader buff, TableRow[] props)
        {
            for(int i=0; i < props.Length; i++)
                props[i] = new Property(buff);
        }

        internal static string[] ReadNames(PEReader buff, ClassDef theClass, uint classIx)
        {
            return Feature.GetFeatureNames(buff, MDTable.Property, MDTable.PropertyMap, theClass, classIx);
        }

        internal sealed override void Resolve(PEReader buff)
        {
            buff.ReadPropertySig(typeBlobIx, this);
        }

        /// <summary>
        /// Add an initial value for this property
        /// </summary>
        /// <param name="constVal">the initial value for this property</param>
        public void AddInitValue(Constant constVal)
        {
            this.constVal = constVal;
        }
        public Constant GetInitValue()
        {
            return constVal;
        }
        public void RemoveInitValue()
        {
            constVal = null;
        }


        public Type GetPropertyType()
        {
            return returnType;
        }
        public void SetPropertyType(Type pType)
        {
            returnType = pType;
        }
        public Type[] GetPropertyParams()
        {
            return parList;
        }
        public void SetPropertyParams(Type[] parTypes)
        {
            parList = parTypes;
        }


        internal void ChangeRefsToDefs(ClassDef newType, ClassDef[] oldTypes)
        {
            throw new NotYetImplementedException("Merge for Properties");
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Property, this);
            nameIx = md.AddToStringsHeap(name);
            for(int i=0; i < numPars; i++)
                parList[i].BuildMDTables(md);
            for(int i=0; i < tide; i++)
                methods[i].BuildMDTables(md);
            if(constVal != null) {
                ConstantElem constElem = new ConstantElem(this, constVal);
                constElem.BuildMDTables(md);
            }
        }

        internal sealed override void BuildSignatures(MetaDataOut md)
        {
            MemoryStream sig = new MemoryStream();
            sig.WriteByte(PropertyTag);
            MetaDataOut.CompressNum(numPars, sig);
            returnType.TypeSig(sig);
            for(int i=0; i < numPars; i++) {
                parList[i].BuildSignatures(md);
                parList[i].TypeSig(sig);
            }
            typeBlobIx = md.AddToBlobHeap(sig.ToArray());
            done = false;
        }

        internal static uint Size(MetaData md)
        {
            return 2 + md.StringsIndexSize() + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(flags);
            output.StringsIndex(nameIx);
            output.BlobIndex(typeBlobIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 9;
            case (CIx.HasConstant):
                return 2;
            case (CIx.HasSemantics):
                return 1;
            }
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for layout information for a field
    /// </summary>
    public class FieldLayout : MetaDataElement {
        FieldDef field;
        uint offset, fieldIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal FieldLayout(FieldDef field, uint offset)
        {
            this.field = field;
            this.offset = offset;
            tabIx = MDTable.FieldLayout;
        }

        internal FieldLayout(PEReader buff)
        {
            offset = buff.ReadUInt32();
            fieldIx = buff.GetIndex(MDTable.Field);
            tabIx = MDTable.FieldLayout;
        }

        internal static void Read(PEReader buff, TableRow[] layouts)
        {
            for(int i=0; i < layouts.Length; i++)
                layouts[i] = new FieldLayout(buff);
        }

        internal sealed override void Resolve(PEReader buff)
        {
            field = (FieldDef)buff.GetElement(MDTable.Field, fieldIx);
            field.SetOffset(offset);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.FieldLayout, this);
        }

        internal static uint Size(MetaData md)
        {
            return 4 + md.TableIndexSize(MDTable.Field);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(offset);
            output.WriteIndex(MDTable.Field, field.Row);
        }

    }
    /*****************************************************************************/
    /// <summary>
    /// Marshalling information for a field or param
    /// </summary>
    public class FieldMarshal : MetaDataElement {
        MetaDataElement field;
        NativeType nt;
        uint ntIx, parentIx;

        /*-------------------- Constructors ---------------------------------*/

        internal FieldMarshal(MetaDataElement field, NativeType nType)
        {
            this.field = field;
            this.nt = nType;
            sortTable = true;
            tabIx = MDTable.FieldMarshal;
        }

        internal FieldMarshal(PEReader buff)
        {
            parentIx = buff.GetCodedIndex(CIx.HasFieldMarshal);
            ntIx = buff.GetBlobIx();
            sortTable = true;
            tabIx = MDTable.FieldMarshal;
        }

        internal static void Read(PEReader buff, TableRow[] fMarshal)
        {
            for(int i=0; i < fMarshal.Length; i++)
                fMarshal[i] = new FieldMarshal(buff);
        }

        internal override void Resolve(PEReader buff)
        {
            field = buff.GetCodedElement(CIx.HasFieldMarshal, parentIx);
            nt = buff.GetBlobNativeType(ntIx);
            if(field is FieldDef) {
                ((FieldDef)field).SetMarshalType(nt);
            } else {
                ((Param)field).SetMarshalType(nt);
            }
        }

        internal override uint SortKey()
        {
            return (field.Row << MetaData.CIxShiftMap[(uint)CIx.HasFieldMarshal]) 
        | field.GetCodedIx(CIx.HasFieldMarshal);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.FieldMarshal, this);
            ntIx = md.AddToBlobHeap(nt.ToBlob());
        }

        internal static uint Size(MetaData md)
        {
            return md.CodedIndexSize(CIx.HasFieldMarshal) + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteCodedIndex(CIx.HasFieldMarshal, field);
            output.BlobIndex(ntIx);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for the address of a field's value in the PE file
    /// </summary>
    public class FieldRVA : MetaDataElement {
        FieldDef field;
        DataConstant data;
        uint rva = 0, fieldIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal FieldRVA(FieldDef field, DataConstant data)
        {
            this.field = field;
            this.data = data;
            tabIx = MDTable.FieldRVA;
        }

        internal FieldRVA(PEReader buff)
        {
            rva = buff.ReadUInt32();
            fieldIx = buff.GetIndex(MDTable.Field);
            tabIx = MDTable.FieldRVA;
        }

        internal static void Read(PEReader buff, TableRow[] fRVAs)
        {
            for(int i=0; i < fRVAs.Length; i++)
                fRVAs[i] = new FieldRVA(buff);
        }

        internal sealed override void Resolve(PEReader buff)
        {
            field = (FieldDef)buff.GetElement(MDTable.Field, fieldIx);
            field.AddDataValue(buff.GetDataConstant(rva, field.GetFieldType()));
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.FieldRVA, this);
            md.AddData(data);
        }

        internal static uint Size(MetaData md)
        {
            return 4 + md.TableIndexSize(MDTable.Field);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteDataRVA(data.DataOffset);
            output.WriteIndex(MDTable.Field, field.Row);
        }

    }
    /**************************************************************************/
    public abstract class FileRef : MetaDataElement {
        protected static readonly uint HasMetaData = 0x0;
        protected static readonly uint HasNoMetaData = 0x1;
        protected uint nameIx = 0, hashIx = 0;
        protected byte[] hashBytes;
        protected string name;
        protected bool entryPoint = false;
        protected uint flags;

        /*-------------------- Constructors ---------------------------------*/

        internal FileRef(string name, byte[] hashBytes)
        {
            this.hashBytes = hashBytes;
            this.name = name;
            tabIx = MDTable.File;
        }

        internal FileRef(PEReader buff)
        {
            flags = buff.ReadUInt32();
            name = buff.GetString();
            hashBytes = buff.GetBlob();
            tabIx = MDTable.File;
        }

        internal static void Read(PEReader buff, TableRow[] files)
        {
            for(int i=0; i < files.Length; i++) {
                uint flags = buff.ReadUInt32();
                if(flags == HasMetaData)
                    files[i] = new ModuleFile(buff.GetString(), buff.GetBlob());
                else
                    files[i] = new ResourceFile(buff.GetString(), buff.GetBlob());
            }
        }

        public string Name()
        {
            return name;
        }
        public byte[] GetHash()
        {
            return hashBytes;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.File, this);
            nameIx = md.AddToStringsHeap(name);
            hashIx = md.AddToBlobHeap(hashBytes);
            if(entryPoint)
                md.SetEntryPoint(this);
        }

        internal static uint Size(MetaData md)
        {
            return 4 + md.StringsIndexSize() + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(flags);
            output.StringsIndex(nameIx);
            output.BlobIndex(hashIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 16;
            case (CIx.Implementation):
                return 0;
            }
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a file referenced in THIS assembly/module (.file)
    /// </summary>
    internal class ModuleFile : FileRef {
        internal ModuleRef fileModule;

        internal ModuleFile(string name, byte[] hashBytes, bool entryPoint)
            : base(name, hashBytes)
        {
            flags = HasMetaData;
            this.entryPoint = entryPoint;
        }

        internal ModuleFile(string name, byte[] hashBytes)
            : base(name, hashBytes)
        {
            flags = HasMetaData;
        }

        internal void SetEntryPoint()
        {
            entryPoint = true;
        }

        internal void SetHash(byte[] hashVal)
        {
            hashBytes = hashVal;
        }


    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a file containing a managed resource
    /// </summary>
    public class ResourceFile : FileRef {
        static ArrayList files = new ArrayList();

        /*-------------------- Constructors ---------------------------------*/

        public ResourceFile(string name, byte[] hashValue)
            : base(name, hashValue)
        {
            flags = HasNoMetaData;
            files.Add(this);
        }

        public static ResourceFile GetFile(string name)
        {
            for(int i=0; i < files.Count; i++) {
                if(((ResourceFile)files[i]).name.Equals(name))
                    return (ResourceFile)files[i];
            }
            return null;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for  
    /// </summary>
    public class GenericParam : Type {
        private static readonly byte VAR = 0x13;
        private static readonly byte MVAR = 0x1E;
        ushort flags, index, kind = 0;
        uint parentIx, nameIx;
        string name;
        MetaDataElement parent;
        private ArrayList constraints = new ArrayList();
        internal static bool extraField = true;

        /*-------------------- Constructors ---------------------------------*/

        private GenericParam(uint index, byte elemIx)
            : base(elemIx)
        {
            this.index = (ushort)index;
            sortTable = true;
        }

        internal GenericParam(string name, MetaDataElement parent, int index)
            : base(VAR)
        {
            this.name = name;
            this.parent = parent;
            this.index = (ushort)index;
            if(parent is Method)
                typeIndex = MVAR;
            sortTable = true;
            tabIx = MDTable.GenericParam;
        }

        internal GenericParam(PEReader buff)
            : base(VAR)
        {
            index = buff.ReadUInt16();
            flags = buff.ReadUInt16();
            parentIx = buff.GetCodedIndex(CIx.TypeOrMethodDef);
            name = buff.GetString();
            if(extraField)
                kind = buff.ReadUInt16();
            sortTable = true;
            tabIx = MDTable.GenericParam;
            // resolve generic param immediately for signature resolution
            parent = buff.GetCodedElement(CIx.TypeOrMethodDef, parentIx);
            if(parent != null) {
                if(parent is MethodDef) {
                    typeIndex = MVAR;
                    ((MethodDef)parent).AddGenericParam(this);
                } else {
                    ((ClassDef)parent).AddGenericParam(this);
                }
            }
        }

        internal GenericParam(string name)
            : base(MVAR)
        {
            this.name = name;
            sortTable = true;
            tabIx = MDTable.GenericParam;
        }

        internal static GenericParam AnonMethPar(uint ix)
        {
            return new GenericParam(ix, MVAR);
        }

        internal static GenericParam AnonClassPar(uint ix)
        {
            return new GenericParam(ix, VAR);
        }

        internal static void Read(PEReader buff, TableRow[] gpars)
        {
            for(int i=0; i < gpars.Length; i++)
                gpars[i] = new GenericParam(buff);
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Set the attribute for this generic parameter
        /// </summary>
        /// <param name="attr">the attribute</param>
        public void SetAttribute(GenericParamAttr attr)
        {
            flags = (ushort)attr;
        }

        /// <summary>
        /// Get the attribute for this generic parameter
        /// </summary>
        public GenericParamAttr GetAttribute()
        {
            return (GenericParamAttr)flags;
        }

        /// <summary>
        /// Add a type constraint to this generic parameter
        /// </summary>
        /// <param name="cType">class constraining the parameter type</param>
        public void AddConstraint(Class cType)
        {
            constraints.Add(cType);
        }

        /// <summary>
        /// Remove a constraint from this generic parameter
        /// </summary>
        /// <param name="cType">class type of constraint</param>
        public void RemoveConstraint(Class cType)
        {
            for(int i=0; i < constraints.Count; i++) {
                if(constraints[i] == cType) {
                    constraints.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Return a constraint from the list
        /// </summary>
        /// <param name="i">constraint index</param>
        /// <returns></returns>
        public Class GetConstraint(int i)
        {
            return (Class)constraints[i];
        }

        /// <summary>
        /// Get the number of constrains on this GenericParam
        /// </summary>
        /// <returns></returns>
        public int GetConstraintCount()
        {
            return constraints.Count;
        }

        /// <summary>
        /// Get the name of this generic parameter
        /// </summary>
        /// <returns>generic parameter name</returns>
        public string GetName()
        {
            return name;
        }

        public MetaDataElement GetParent()
        {
            return parent;
        }

        public Class[] GetClassConstraints()
        {
            return (Class[])constraints.ToArray(typeof(Class)); // KJG 20-May-2005
        }

        /*----------------------------- internal functions ------------------------------*/

        internal uint Index
        {
            get
            {
                return index;
            }
            set
            {
                index = (ushort)value;
            }
        }

        internal void SetClassParam(Class paren, int ix)
        {
            typeIndex = VAR;
            parent = paren;
            index = (ushort)ix;
        }

        internal void SetMethParam(Method paren, int ix)
        {
            typeIndex = MVAR;
            parent = paren;
            index = (ushort)ix;
        }

        internal void CheckParent(MethodDef paren, PEReader buff)
        {
            if(paren == buff.GetCodedElement(CIx.TypeOrMethodDef, parentIx)) {
                parent = paren;
                paren.InsertGenericParam(this);
            }
        }

        internal override void TypeSig(MemoryStream str)
        {
            str.WriteByte(typeIndex);
            str.WriteByte((byte)index);
        }

        internal static uint Size(MetaData md)
        {
            if(extraField)
                return 6 + md.CodedIndexSize(CIx.TypeOrMethodDef) + md.StringsIndexSize();
            else
                return 4 + md.CodedIndexSize(CIx.TypeOrMethodDef) + md.StringsIndexSize();
        }

        internal override Type AddTypeSpec(MetaDataOut md)
        {
            // check that this generic parameter belongs to the "current" method ??
            GenericParTypeSpec tSpec = new GenericParTypeSpec(this);
            md.AddToTable(MDTable.TypeSpec, tSpec);
            return tSpec;
        }

        internal override uint SortKey()
        {
            return (parent.Row << MetaData.CIxShiftMap[(uint)CIx.TypeOrMethodDef]) 
        | parent.GetCodedIx(CIx.TypeOrMethodDef);
        }

        internal override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.GenericParam, this);
            nameIx = md.AddToStringsHeap(name);
            for(int i=0; i < constraints.Count; i++) {
                Class cClass = (Class)constraints[i];
                constraints[i] = new GenericParamConstraint(this, cClass);
                if(cClass is ClassRef)
                    cClass.BuildMDTables(md);
            }
        }

        internal void AddConstraints(MetaDataOut md)
        {
            for(int i=0; i < constraints.Count; i++) {
                md.AddToTable(MDTable.GenericParamConstraint, (GenericParamConstraint)constraints[i]);
            }
        }

        internal override void Write(PEWriter output)
        {
            output.Write(index);
            output.Write(flags);
            output.WriteCodedIndex(CIx.TypeOrMethodDef, parent);
            output.StringsIndex(nameIx);
            if(extraField)
                output.Write(kind);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for GenericParamConstraint 
    /// </summary>
    public class GenericParamConstraint : MetaDataElement {
        uint parentIx, constraintIx;
        GenericParam parent;
        Class constraint;

        /*-------------------- Constructors ---------------------------------*/

        public GenericParamConstraint(GenericParam parent, Class constraint)
        {
            this.parent = parent;
            this.constraint = constraint;
            tabIx = MDTable.GenericParamConstraint;
        }

        internal GenericParamConstraint(PEReader buff)
        {
            parentIx = buff.GetIndex(MDTable.GenericParam);
            constraintIx = buff.GetCodedIndex(CIx.TypeDefOrRef);
            tabIx = MDTable.GenericParam;
        }

        internal static void Read(PEReader buff, TableRow[] gpars)
        {
            for(int i=0; i < gpars.Length; i++)
                gpars[i] = new GenericParamConstraint(buff);
        }

        internal override void Resolve(PEReader buff)
        {
            parent = (GenericParam)buff.GetElement(MDTable.GenericParam, parentIx);
            parent.AddConstraint((Class)buff.GetCodedElement(CIx.TypeDefOrRef, constraintIx));
        }

        internal static uint Size(MetaData md)
        {
            return md.TableIndexSize(MDTable.GenericParam) + md.CodedIndexSize(CIx.TypeDefOrRef);
        }

        internal override void Write(PEWriter output)
        {
            output.WriteIndex(MDTable.GenericParam, parent.Row);
            output.WriteCodedIndex(CIx.TypeDefOrRef, constraint);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for an Instantiation of a generic method 
    /// </summary>
    public class MethodSpec : Method {
        Method methParent;
        uint instIx;
        Type[] instTypes;
        internal static byte GENERICINST = 0x0A;

        /*-------------------- Constructors ---------------------------------*/

        public MethodSpec(Method mParent, Type[] instTypes)
            : base(null)
        {
            this.methParent = mParent;
            this.instTypes = instTypes;
            tabIx = MDTable.MethodSpec;
        }

        internal MethodSpec(PEReader buff)
            : base(null)
        {
            parentIx = buff.GetCodedIndex(CIx.MethodDefOrRef);
            instIx = buff.GetBlobIx();
            tabIx = MDTable.MethodSpec;
            this.unresolved = true;
        }

        internal static void Read(PEReader buff, TableRow[] specs)
        {
            for(int i=0; i < specs.Length; i++)
                specs[i] = new MethodSpec(buff);
        }

        internal override void Resolve(PEReader buff)
        {
            methParent = (Method)buff.GetCodedElement(CIx.MethodDefOrRef, parentIx);
            instTypes = buff.ReadMethSpecSig(instIx);
            this.unresolved = false;
        }

        internal override void TypeSig(MemoryStream str)
        {
            str.WriteByte(GENERICINST);
            MetaDataOut.CompressNum((uint)instTypes.Length, str);
            for(int i=0; i < instTypes.Length; i++) {
                instTypes[i].TypeSig(str);
            }
        }

        internal static uint Size(MetaData md)
        {
            return md.CodedIndexSize(CIx.MethodDefOrRef) + md.BlobIndexSize();
        }

        internal override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.MethodSpec, this);
            methParent.BuildMDTables(md);
            for(int i=0; i < instTypes.Length; i++) {
                instTypes[i].BuildMDTables(md);
            }
        }

        internal override void BuildSignatures(MetaDataOut md)
        {
            MemoryStream outSig = new MemoryStream();
            TypeSig(outSig);
            instIx = md.AddToBlobHeap(outSig.ToArray());
        }

        internal override void Write(PEWriter output)
        {
            output.WriteCodedIndex(CIx.MethodDefOrRef, methParent);
            output.BlobIndex(instIx);
        }

        /*-------------------- Public Methods ------------------------------*/

        public Type[] GetGenericParamTypes()
        {  // KJG 15 July 2005
            return instTypes;
        }

        public Method GetMethParent()
        {         // KJG 15 July 2005
            return methParent;
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for pinvoke information for a method 
    /// </summary>
    public class ImplMap : MetaDataElement {
        private static readonly ushort NoMangle = 0x01;
        ushort flags;
        MethodDef meth;
        string importName;
        uint iNameIx, scopeIx = 0, memForIndex = 0;
        ModuleRef importScope;

        /*-------------------- Constructors ---------------------------------*/

        internal ImplMap(ushort flag, MethodDef implMeth, string iName, ModuleRef mScope)
        {
            flags = flag;
            meth = implMeth;
            importName = iName;
            importScope = mScope;
            tabIx = MDTable.ImplMap;
            if(iName == null)
                flags |= NoMangle;
            sortTable = true;
            //throw(new NotYetImplementedException("PInvoke "));
        }

        internal ImplMap(PEReader buff)
        {
            flags = buff.ReadUInt16();
            memForIndex = buff.GetCodedIndex(CIx.MemberForwarded);
            importName = buff.GetString();
            scopeIx = buff.GetIndex(MDTable.ModuleRef);
            sortTable = true;
            tabIx = MDTable.ImplMap;
        }

        internal static void Read(PEReader buff, TableRow[] impls)
        {
            for(int i=0; i < impls.Length; i++)
                impls[i] = new ImplMap(buff);
        }

        internal override void Resolve(PEReader buff)
        {
            meth = (MethodDef)buff.GetCodedElement(CIx.MemberForwarded, memForIndex);
            importScope = (ModuleRef)buff.GetElement(MDTable.ModuleRef, scopeIx);
            if(meth != null)
                meth.AddPInvokeInfo(this);
        }

        internal override uint SortKey()
        {
            return (meth.Row << MetaData.CIxShiftMap[(uint)CIx.MemberForwarded]) 
        | meth.GetCodedIx(CIx.MemberForwarded);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.ImplMap, this);
            iNameIx = md.AddToStringsHeap(importName);
            importScope.BuildMDTables(md);
        }

        internal static uint Size(MetaData md)
        {
            return 2+ md.CodedIndexSize(CIx.MemberForwarded) + 
        md.StringsIndexSize() +  md.TableIndexSize(MDTable.ModuleRef);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(flags);
            output.WriteCodedIndex(CIx.MemberForwarded, meth);
            output.StringsIndex(iNameIx);
            output.WriteIndex(MDTable.ModuleRef, importScope.Row);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for interface implemented by a class
    /// </summary>
    public class InterfaceImpl : MetaDataElement {
        ClassDef theClass;
        Class theInterface;
        uint classIx = 0, interfacesIndex = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal InterfaceImpl(ClassDef theClass, Class theInterface)
        {
            this.theClass = theClass;
            this.theInterface = theInterface;
            tabIx = MDTable.InterfaceImpl;
        }

        internal InterfaceImpl(ClassDef theClass, TableRow theInterface)
        {
            this.theClass = theClass;
            this.theInterface = (Class)theInterface;
            tabIx = MDTable.InterfaceImpl;
        }

        internal InterfaceImpl(PEReader buff)
        {
            classIx = buff.GetIndex(MDTable.TypeDef);
            interfacesIndex = buff.GetCodedIndex(CIx.TypeDefOrRef);
            tabIx = MDTable.InterfaceImpl;
        }

        internal override void Resolve(PEReader buff)
        {
            theClass = (ClassDef)buff.GetElement(MDTable.TypeDef, classIx);
            theInterface = (Class)buff.GetCodedElement(CIx.TypeDefOrRef, interfacesIndex);
            theClass.AddImplementedInterface(this);
        }

        internal static void Read(PEReader buff, TableRow[] impls)
        {
            for(int i=0; i < impls.Length; i++)
                impls[i] = new InterfaceImpl(buff);
        }

        internal ClassDef TheClass()
        {
            return theClass;
        }
        internal Class TheInterface()
        {
            return theInterface;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.InterfaceImpl, this);
            if(!theInterface.isDef())
                theInterface.BuildMDTables(md);
            if(theInterface is ClassSpec)
                md.AddToTable(MDTable.TypeSpec, theInterface);
        }

        internal static uint Size(MetaData md)
        {
            return md.TableIndexSize(MDTable.TypeDef) + 
        md.CodedIndexSize(CIx.TypeDefOrRef);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteIndex(MDTable.TypeDef, theClass.Row);
            output.WriteCodedIndex(CIx.TypeDefOrRef, theInterface);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            return 5;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for resources used in this PE file NOT YET IMPLEMENTED
    /// </summary>
    public class ManifestResource : MetaDataElement {
        private static readonly uint PublicResource = 0x1;
        private static readonly uint PrivateResource = 0x2;

        string mrName;
        MetaDataElement impl;  // can be AssemblyRef, ResourceFile or ModuleFile
        uint fileOffset = 0;
        uint nameIx = 0, implIx = 0;
        uint flags = 0;
        PEFile pefile;
        byte[] resourceBytes;

        /*-------------------- Constructors ---------------------------------*/

        internal ManifestResource(PEFile pefile, string name, byte[] resBytes, bool isPub)
        {
            InitResource(pefile, name, isPub);
            this.resourceBytes = resBytes;
        }

        internal ManifestResource(PEFile pefile, string name, MetaDataElement fileRef, uint offset, bool isPub)
        {
            InitResource(pefile, name, isPub);
            impl = fileRef;
            fileOffset = offset;
        }

        internal ManifestResource(PEFile pefile, ManifestResource mres, bool isPub)
        {
            this.pefile = pefile;
            mrName = mres.mrName;
            flags = mres.flags;
            this.impl = mres.impl;
            this.fileOffset = mres.fileOffset;
            this.resourceBytes = mres.resourceBytes;
        }

        internal ManifestResource(PEReader buff)
        {
            fileOffset = buff.ReadUInt32();
            flags = buff.ReadUInt32();
            mrName = buff.GetString();
            implIx = buff.GetCodedIndex(CIx.Implementation);
            tabIx = MDTable.ManifestResource;
        }

        private void InitResource(PEFile pefile, string name, bool isPub)
        {
            this.pefile = pefile;
            mrName = name;
            if(isPub)
                flags = PublicResource;
            else
                flags = PrivateResource;
            tabIx = MDTable.ManifestResource;
        }

        internal static void Read(PEReader buff, TableRow[] mrs)
        {
            for(int i=0; i < mrs.Length; i++)
                mrs[i] = new ManifestResource(buff);
        }

        internal override void Resolve(PEReader buff)
        {
            impl = buff.GetCodedElement(CIx.Implementation, implIx);
            if(impl == null)
                resourceBytes = buff.GetResource(fileOffset);
        }

        /*------------------------- public set and get methods --------------------------*/

        public string Name
        {
            get
            {
                return mrName;
            }
            set
            {
                mrName = value;
            }
        }

        public byte[] ResourceBytes
        {
            get
            {
                return resourceBytes;
            }
            set
            {
                resourceBytes = value;
            }
        }

        public AssemblyRef ResourceAssembly
        {
            get
            {
                if(impl is AssemblyRef)
                    return (AssemblyRef)impl;
                return null;
            }
            set
            {
                impl = value;
            }
        }

        public ResourceFile ResFile
        {
            get
            {
                if(impl is ResourceFile)
                    return (ResourceFile)impl;
                return null;
            }
            set
            {
                impl = value;
            }
        }

        public ModuleRef ResourceModule
        {
            get
            {
                if(impl is ModuleFile)
                    return ((ModuleFile)impl).fileModule;
                return null;
            }
            set
            {
                impl = value.modFile;
            }
        }

        public uint FileOffset
        {
            get
            {
                return fileOffset;
            }
            set
            {
                fileOffset = value;
            }
        }

        public bool IsPublic
        {
            get
            {
                return flags == PublicResource;
            }
            set
            {
                if(value)
                    flags = PublicResource;
                else
                    flags = PrivateResource;
            }
        }

        /*----------------------------- internal functions ------------------------------*/

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.ManifestResource, this);
            nameIx = md.AddToStringsHeap(mrName);
            if(resourceBytes != null) {
                if(impl != null)
                    throw new Exception("ERROR:  Manifest Resource has byte value and file reference");
                fileOffset = md.AddResource(resourceBytes);
            } else {
                if(impl == null)
                    throw new Exception("ERROR:  Manifest Resource has no implementation or value");
                impl.BuildMDTables(md);
            }
        }

        internal static uint Size(MetaData md)
        {
            return 8 + md.StringsIndexSize() + 
        md.CodedIndexSize(CIx.Implementation);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(fileOffset);
            output.Write(flags);
            output.StringsIndex(nameIx);
            output.WriteCodedIndex(CIx.Implementation, impl);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            return 18;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Base class for elements in the PropertyMap, EventMap and 
    /// NestedClass MetaData tables
    /// </summary>
    public class MapElem : MetaDataElement {
        ClassDef theClass, parent;
        uint elemIx, classIx, endIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal MapElem(ClassDef classDef, uint elIx, MDTable tableIx)
        {
            theClass = classDef;
            elemIx = elIx;
            tabIx = tableIx;
        }

        internal MapElem(ClassDef classDef, ClassDef paren, MDTable tableIx)
        {
            theClass = classDef;
            parent = paren;
            tabIx = tableIx;
        }

        internal MapElem(PEReader buff, MDTable tab)
        {
            tabIx = tab;
            classIx = buff.GetIndex(MDTable.TypeDef);
            elemIx = buff.GetIndex(tab);
        }

        internal static void Read(PEReader buff, TableRow[] maps, MDTable tab)
        {
            if(tab == MDTable.NestedClass) {
                for(int i=0; i < maps.Length; i++) {
                    //maps[i] = new MapElem(buff,tab);
                    uint nestClassIx = buff.GetIndex(MDTable.TypeDef);
                    uint enclClassIx = buff.GetIndex(MDTable.TypeDef);
                    ClassDef parent = (ClassDef)buff.GetElement(MDTable.TypeDef, enclClassIx);
                    ClassDef nestClass = ((ClassDef)buff.GetElement(MDTable.TypeDef, nestClassIx)).MakeNestedClass(parent);
                    buff.InsertInTable(MDTable.TypeDef, nestClass.Row, nestClass);
                }
            } else { // event or property map
                MapElem prev = new MapElem(buff, tab);
                maps[0] = prev;
                for(int i=1; i < maps.Length; i++) {
                    maps[i] = new MapElem(buff, tab);
                    prev.endIx = ((MapElem)maps[i]).elemIx;
                    prev = (MapElem)maps[i];
                }
                if(tab == MDTable.PropertyMap)
                    prev.endIx = buff.GetTableSize(MDTable.Property)+1;
                else
                    prev.endIx = buff.GetTableSize(MDTable.Event)+1;
            }
        }

        internal static void ReadNestedClassInfo(PEReader buff, uint num, uint[] parIxs)
        {
            for(int i=0; i < parIxs.Length; i++)
                parIxs[i] = 0;
            for(int i=0; i < num; i++) {
                int ix = (int)buff.GetIndex(MDTable.TypeDef);
                parIxs[ix-1] = buff.GetIndex(MDTable.TypeDef);
            }
        }

        internal override void Resolve(PEReader buff)
        {
            theClass = (ClassDef)buff.GetElement(MDTable.TypeDef, classIx);
            if(tabIx == MDTable.EventMap) {
                for(uint i=elemIx; i < endIx; i++)
                    theClass.AddEvent((Event)buff.GetElement(MDTable.Event, i));
            } else if(tabIx == MDTable.PropertyMap) {
                for(uint i=elemIx; i < endIx; i++)
                    theClass.AddProperty((Property)buff.GetElement(MDTable.Property, i));
            } else { // must be nested class -- already done
                //ClassDef parent = (ClassDef)buff.GetElement(MDTable.TypeDef,elemIx);
                //parent.MakeNested(theClass);
            }
        }

        internal static uint Size(MetaData md, MDTable tabIx)
        {
            return md.TableIndexSize(MDTable.TypeDef) + md.TableIndexSize(tabIx);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(tabIx, this);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteIndex(MDTable.TypeDef, theClass.Row);
            if(parent != null)
                output.WriteIndex(MDTable.TypeDef, parent.Row);
            else
                output.WriteIndex(tabIx, elemIx);
        }
    }
    /**************************************************************************/
    /// <summary>
    /// Base class for field/methods (member of a class)
    /// </summary>
    public abstract class Member : MetaDataElement {
        protected string name;
        protected uint nameIx = 0, sigIx = 0;
        protected byte[] signature;
        protected uint parentIx = 0;
        protected Class parent;

        /*-------------------- Constructors ---------------------------------*/

        internal Member(string memName, Class paren)
        {
            name = memName;
            parent = paren;
            tabIx = MDTable.MemberRef;
        }

        internal Member(uint parenIx, string name, uint sIx)
        {
            parentIx = parenIx;
            this.name = name;
            sigIx = sIx;
            tabIx = MDTable.MemberRef;
        }

        internal Member(string name)
        {
            this.name = name;
            tabIx = MDTable.MemberRef;
        }

        internal static void ReadMember(PEReader buff, TableRow[] members)
        {
            for(int i=0; i < members.Length; i++) {
                uint parenIx = buff.GetCodedIndex(CIx.MemberRefParent);
                string memName = buff.GetString();
                uint sigIx = buff.GetBlobIx();
                if(buff.FirstBlobByte(sigIx) == Field.FieldTag) // got a field
                    members[i] = new FieldRef(parenIx, memName, sigIx);
                else
                    members[i] = new MethodRef(parenIx, memName, sigIx);
            }
        }

        internal virtual Member ResolveParent(PEReader buff)
        {
            return null;
        }

        public MetaDataElement GetParent()
        {
            if(parent == null)
                return null;
            if(parent.isSpecial())
                return parent.GetParent();
            return parent;
        }

        internal void SetParent(Class paren)
        {
            parent = paren;
        }

        public string Name()
        {
            return name;
        }

        public string QualifiedName()
        {
            return parent.TypeName() + "." + name;
        }

        internal bool HasName(string name)
        {
            return (this.name == name);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a field of a class
    /// </summary>
    public abstract class Field : Member {
        internal static readonly byte FieldTag = 0x6;

        protected Type type;

        /*-------------------- Constructors ---------------------------------*/

        internal Field(string pfName, Type pfType, Class paren)
            : base(pfName, paren)
        {
            type = pfType;
        }

        internal override void Resolve(PEReader buff)
        {
            if(type == null) {
                buff.currentClassScope = parent;
                type = buff.GetFieldType(sigIx);
                buff.currentClassScope = null;
            }
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Get the type of this field
        /// </summary>
        /// <returns>Type descriptor for this field</returns>
        public Type GetFieldType()
        {
            return type;
        }

        /// <summary>
        /// Set the type of this field
        /// </summary>
        /// <param name="ty">The type of the field</param>
        public void SetFieldType(Type ty)
        {
            type = ty;
        }

        /*----------------------------- internal functions ------------------------------*/

        internal sealed override void BuildSignatures(MetaDataOut md)
        {
            MemoryStream sig = new MemoryStream();
            sig.WriteByte(FieldTag);
            type.TypeSig(sig);
            sigIx = md.AddToBlobHeap(sig.ToArray());
            done = false;
        }

        internal override string NameString()
        {
            return parent.NameString() + "." + name;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a field defined in a class of an assembly/module
    /// </summary>
    public class FieldDef : Field {
        //private static readonly uint PInvokeImpl = 0x2000;
        private static readonly ushort HasFieldMarshal = 0x1000;
        private static readonly ushort HasFieldRVA = 0x100;
        private static readonly ushort HasDefault = 0x8000;
        private static readonly ushort NoFieldMarshal = 0xEFFF;
        private static readonly ushort NoFieldRVA = 0xFEFF;
        private static readonly ushort NoDefault = 0x7FFF;

        internal FieldRef refOf;
        DataConstant initVal;
        Constant constVal;
        NativeType marshalType;
        ushort flags;
        bool hasOffset = false;
        uint offset;

        /*-------------------- Constructors ---------------------------------*/

        internal FieldDef(string name, Type fType, ClassDef paren)
            : base(name, fType, paren)
        {
            tabIx = MDTable.Field;
        }

        internal FieldDef(FieldAttr attrSet, string name, Type fType, ClassDef paren)
            : base(name, fType, paren)
        {
            flags = (ushort)attrSet;
            tabIx = MDTable.Field;
        }

        internal FieldDef(FieldAttr attrSet, string name, Type fType, ClassSpec paren)
            : base(name, fType, paren)
        {
            flags = (ushort)attrSet;
            tabIx = MDTable.Field;
        }

        internal FieldDef(PEReader buff)
            : base(null, null, null)
        {
            flags = buff.ReadUInt16();
            name = buff.GetString();
            sigIx = buff.GetBlobIx();
            tabIx = MDTable.Field;
        }

        internal static void Read(PEReader buff, TableRow[] fields)
        {
            for(int i=0; i < fields.Length; i++)
                fields[i] = new FieldDef(buff);
        }

        internal static void GetFieldRefs(PEReader buff, uint num, ClassRef parent)
        {
            for(int i=0; i < num; i++) {
                uint flags = buff.ReadUInt16();
                string name = buff.GetString();
                uint sigIx = buff.GetBlobIx();
                if((flags & (uint)FieldAttr.Public) == (uint)FieldAttr.Public) {
                    if(parent.GetField(name) == null) {
                        //Console.WriteLine(parent.NameString());
                        buff.currentClassScope = parent;
                        FieldRef fRef = new FieldRef(parent, name, buff.GetFieldType(sigIx));
                        buff.currentClassScope = null;
                        parent.AddToFieldList(fRef);
                    }
                }
            }
        }

        internal void Resolve(PEReader buff, uint fIx)
        {
            /*
            if ((flags & HasFieldMarshal) != 0) 
              marshalType = FieldMarshal.FindMarshalType(buff,this,
                buff.MakeCodedIndex(CIx.HasFieldMarshal,MDTable.Field,fIx));
            if ((flags & HasFieldRVA) != 0)
              initVal = FieldRVA.FindValue(buff,this,fIx);
            if ((flags & HasDefault) != 0)
              constVal = ConstantElem.FindConst(buff,this,
                buff.MakeCodedIndex(CIx.HasConstant,MDTable.Field,fIx));
            long offs = FieldLayout.FindLayout(buff,this,fIx);
            if (offs > -1){
              hasOffset = true;
              offset = (uint)offs;
            }
            */
            buff.currentClassScope = parent;
            type = buff.GetFieldType(sigIx);
            buff.currentClassScope = null;
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Add an attribute(s) to this field
        /// </summary>
        /// <param name="fa">the attribute(s) to be added</param>
        public void AddFieldAttr(FieldAttr fa)
        {
            flags |= (ushort)fa;
        }
        public void SetFieldAttr(FieldAttr fa)
        {
            flags = (ushort)fa;
        }
        public FieldAttr GetFieldAttr()
        {
            return (FieldAttr)flags;
        }

        /// <summary>
        /// Add a value for this field
        /// </summary>
        /// <param name="val">the value for the field</param>
        public void AddValue(Constant val)
        {
            flags |= HasDefault;
            constVal = val;
        }
        public Constant GetValue()
        {
            return constVal;
        }
        public void RemoveValue()
        {
            constVal = null;
            flags &= NoDefault;
        }

        /// <summary>
        /// Add an initial value for this field (at dataLabel) (.data)
        /// </summary>
        /// <param name="val">the value for the field</param>
        public void AddDataValue(DataConstant val)
        {
            flags |= HasFieldRVA;
            initVal = val;
        }

        /// <summary>
        /// Get the value for this data constant
        /// </summary>
        /// <returns></returns>
        public DataConstant GetDataValue()
        {
            return initVal;
        }

        /// <summary>
        /// Delete the value of this data constant
        /// </summary>
        public void RemoveDataValue()
        {
            initVal = null;
            flags &= NoFieldRVA;
        }

        /// <summary>
        /// Set the offset of the field.  Used for sequential or explicit classes.
        /// (.field [offs])
        /// </summary>
        /// <param name="offs">field offset</param>
        public void SetOffset(uint offs)
        {
            offset = offs;
            hasOffset = true;
        }

        /// <summary>
        /// Return the offset for this data constant
        /// </summary>
        /// <returns></returns>
        public uint GetOffset()
        {
            return offset;
        }

        /// <summary>
        /// Delete the offset of this data constant
        /// </summary>
        public void RemoveOffset()
        {
            hasOffset = false;
        }

        /// <summary>
        /// Does this data constant have an offset?
        /// </summary>
        public bool HasOffset()
        {
            return hasOffset;
        }

        /// <summary>
        /// Set the marshalling info for a field
        /// </summary>
        /// <param name="mType"></param>
        public void SetMarshalType(NativeType mType)
        {
            flags |= HasFieldMarshal;
            marshalType = mType;
        }
        public NativeType GetMarshalType()
        {
            return marshalType;
        }
        public void RemoveMarshalType()
        {
            marshalType = null;
            flags &= NoFieldMarshal;
        }


        public FieldRef RefOf()
        {
            return refOf;
        }

        public FieldRef MakeRefOf()
        {
            if(refOf != null)
                return refOf;
            ClassRef parRef = ((ClassDef)parent).MakeRefOf();
            refOf = parRef.GetField(name);
            if(refOf == null) {
                refOf = new FieldRef(parRef, name, type);
                refOf.defOf = this;
            }
            return refOf;
        }

        /*------------------------- internal functions --------------------------*/

        internal PEFile GetScope()
        {
            return ((ClassDef)parent).GetScope();
        }

        internal void ChangeRefsToDefs(ClassDef newPar, ClassDef[] oldTypes)
        {
            parent = newPar;
            bool changeType = false;
            for(int i=0; i < oldTypes.Length && !changeType; i++) {
                if(type == oldTypes[i])
                    type = newPar;
            }
        }

        internal override bool isDef()
        {
            return true;
        }

        internal void SetParent(ClassDef paren)
        {
            parent = paren;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Field, this);
            nameIx = md.AddToStringsHeap(name);
            if(!type.isDef())
                type.BuildMDTables(md);
            if(initVal != null) {
                FieldRVA rva = new FieldRVA(this, initVal);
                rva.BuildMDTables(md);
            }
            if(constVal != null) {
                ConstantElem constElem = new ConstantElem(this, constVal);
                constElem.BuildMDTables(md);
            }
            if(hasOffset) {
                FieldLayout layout = new FieldLayout(this, offset);
                layout.BuildMDTables(md);
            }
            if(marshalType != null) {
                FieldMarshal marshalInfo = new FieldMarshal(this, marshalType);
                marshalInfo.BuildMDTables(md);
            }
        }

        internal static uint Size(MetaData md)
        {
            return 2 + md.StringsIndexSize() + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(flags);
            output.StringsIndex(nameIx);
            output.BlobIndex(sigIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasConstant):
                return 0;
            case (CIx.HasCustomAttr):
                return 1;
            case (CIx.HasFieldMarshal):
                return 0;
            case (CIx.MemberForwarded):
                return 0;
            }
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a field of a class defined in another assembly/module
    /// </summary>
    public class FieldRef : Field {
        internal FieldDef defOf;

        /*-------------------- Constructors ---------------------------------*/

        internal FieldRef(Class paren, string name, Type fType)
            : base(name, fType, paren)
        {
            parent = paren;
        }

        internal FieldRef(uint parenIx, string name, uint sigIx)
            : base(name, null, null)
        {
            parentIx = parenIx;
            this.name = name;
            this.sigIx = sigIx;
        }

        internal override Member ResolveParent(PEReader buff)
        {
            if(parent != null)
                return this;
            MetaDataElement paren = buff.GetCodedElement(CIx.MemberRefParent, parentIx);
            //Console.WriteLine("parentIx = " + parentIx);
            //Console.WriteLine("paren = " + paren);
            if(paren is ClassDef)
                return ((ClassDef)paren).GetField(this.name);
            //if (paren is ClassSpec)
            // paren = ((ClassSpec)paren).GetParent();
            if(paren is ReferenceScope)
                parent = ((ReferenceScope)paren).GetDefaultClass();
            if(paren is TypeSpec)
                parent = new ConstructedTypeSpec((TypeSpec)paren);
            else
                parent = (Class)paren;
            if(parent != null) {
                Field existing = (Field)((Class)parent).GetFieldDesc(name);
                if(existing != null) {
                    return existing;
                }
            }
            parent.AddToFieldList(this);
            return this;
        }

        /*------------------------- internal functions --------------------------*/

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(tabIx, this);
            nameIx = md.AddToStringsHeap(name);
            type.BuildMDTables(md);
            if(parent != null) {
                if(parent is ClassSpec)
                    md.AddToTable(MDTable.TypeSpec, parent);
                parent.BuildMDTables(md);
            }
        }

        internal static uint Size(MetaData md)
        {
            return md.CodedIndexSize(CIx.MemberRefParent) + md.StringsIndexSize() + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteCodedIndex(CIx.MemberRefParent, parent);
            output.StringsIndex(nameIx);
            output.BlobIndex(sigIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            return 6;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Base class for Method Descriptors
    /// </summary>
    public abstract class Method : Member {
        protected MethSig sig;
        protected ArrayList genericParams = new ArrayList();

        /*-------------------- Constructors ---------------------------------*/

        internal Method(string methName, Type rType, Class paren)
            : base(methName, paren)
        {
            sig = new MethSig(methName);
            sig.retType = rType;
        }

        internal Method(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Add calling conventions to this method descriptor
        /// </summary>
        /// <param name="cconv"></param>
        public void AddCallConv(CallConv cconv)
        {
            sig.callConv |= cconv;
        }

        /// <summary>
        /// Get the calling conventions for this method
        /// </summary>
        /// <returns></returns>
        public CallConv GetCallConv()
        {
            return sig.callConv;
        }

        /// <summary>
        /// Set the return type
        /// </summary>
        /// <param name="retT">type returned</param>
        internal void AddRetType(Type retT)
        {
            System.Diagnostics.Debug.Assert(retT != null);
            sig.retType = retT;
        }

        /// <summary>
        /// Get the method return type
        /// </summary>
        /// <returns>method return type</returns>
        public Type GetRetType()
        {
            return sig.retType;
        }

        /// <summary>
        /// Get the types of the method parameters
        /// </summary>
        /// <returns>list of parameter types</returns>
        public Type[] GetParTypes()
        {
            return sig.parTypes;
        }

        /// <summary>
        /// Get the optional parameter types (for varargs)
        /// </summary>
        /// <returns>list of vararg types</returns>
        public Type[] GetOptParTypes()
        {
            return sig.optParTypes;
        }

        public int GetGenericParamCount()
        {
            return genericParams.Count;
        }

        /// <summary>
        /// Add a generic type to this method
        /// </summary>
        /// <param name="name">the name of the generic type</param>
        /// <returns>the descriptor for the generic type</returns>
        public GenericParam AddGenericParam(string name)
        {
            GenericParam gp = new GenericParam(name, this, genericParams.Count);
            sig.callConv |= CallConv.Generic;
            genericParams.Add(gp);
            sig.numGenPars = (uint)genericParams.Count;
            return gp;
        }

        /// <summary>
        /// Get the descriptor for a generic type 
        /// </summary>
        /// <param name="name">the name of the generic type</param>
        /// <returns>descriptor for generic type "name"</returns>
        public GenericParam GetGenericParam(string name)
        {
            int pos = FindGenericParam(name);
            if(pos == -1)
                return null;
            return (GenericParam)genericParams[pos];
        }

        public GenericParam GetGenericParam(int ix)
        {
            if(ix >= genericParams.Count)
                return null;
            return (GenericParam)genericParams[ix];
        }

        public void RemoveGenericParam(string name)
        {
            int pos = FindGenericParam(name);
            if(pos == -1)
                return;
            DeleteGenericParam(pos);
        }

        public void RemoveGenericParam(int ix)
        {
            if(ix >= genericParams.Count)
                return;
            DeleteGenericParam(ix);
        }

        public MethodSpec Instantiate(Type[] genTypes)
        {
            if(genTypes == null)
                return null;
            if(genericParams.Count == 0)
                throw new Exception("Cannot instantiate non-generic method");
            if(genTypes.Length != genericParams.Count)
                throw new Exception("Wrong number of type parameters for instantiation\nNeeded " 
          + genericParams.Count + " but got " + genTypes.Length);
            return new MethodSpec(this, genTypes);
        }

        public GenericParam[] GetGenericParams()
        {    // KJG June 2005
            return (GenericParam[])genericParams.ToArray(typeof(GenericParam));
        }

        /*------------------------- internal functions --------------------------*/

        internal abstract void TypeSig(MemoryStream sig);

        internal bool HasNameAndSig(string name, Type[] sigTypes)
        {
            if(this.name != name)
                return false;
            return sig.HasSig(sigTypes);
        }

        internal bool HasNameAndSig(string name, Type[] sigTypes, Type[] optPars)
        {
            if(this.name != name)
                return false;
            return sig.HasSig(sigTypes, optPars);
        }

        internal MethSig GetSig()
        {
            return sig;
        }

        internal void SetSig(MethSig sig)
        {
            this.sig = sig;
            this.sig.name = name;
        }

        internal override string NameString()
        {
            return parent.NameString() + sig.NameString();
        }

        private int FindGenericParam(string name)
        {
            for(int i=0; i < genericParams.Count; i++) {
                GenericParam gp = (GenericParam)genericParams[i];
                if(gp.GetName() == name)
                    return i;
            }
            return -1;
        }

        private void DeleteGenericParam(int pos)
        {
            genericParams.RemoveAt(pos);
            for(int i=pos; i < genericParams.Count; i++) {
                GenericParam gp = (GenericParam)genericParams[i];
                gp.Index = (uint)i;
            }
        }

        internal void AddGenericParam(GenericParam par)
        {
            genericParams.Add(par);
            //sig.callConv |= CallConv.Generic;
            //sig.numGenPars = (uint)genericParams.Count;
        }

        internal ArrayList GenericParams
        {
            get
            {
                return genericParams;
            }
            set
            {
                genericParams = value;
            }
        }

        internal void SetGenericParams(GenericParam[] pars)
        {
            genericParams = new ArrayList(pars);
            sig.callConv |= CallConv.Generic;
            sig.numGenPars = (uint)genericParams.Count;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a method defined in THIS assembly/module
    /// IL     .method
    /// </summary>
    public class MethodDef : Method {
        private static readonly ushort PInvokeImpl = 0x2000;
        private static readonly ushort NotPInvoke  = 0xDFFF;
        private static readonly ushort HasSecurity = 0x4000;
        private static readonly ushort NoSecurity  = 0xBFFF;
        //private static readonly uint UnmanagedExport = 0x0008;
        uint parIx = 0, textOffset = 0;
        internal MethodRef refOf;

        CILInstructions code;
        uint rva;
        Param[] parList;
        Local[] locals;
        bool initLocals;
        ushort methFlags = 0, implFlags = 0;
        int maxStack = 0, numLocals = 0;
        uint numPars = 0;
        bool entryPoint = false;
        internal LocalSig localSig;
        MethodRef varArgSig;
        ImplMap pinvokeImpl;
        ArrayList security = null;
        internal uint locToken = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal MethodDef(string name, Type retType, Param[] pars, ClassDef paren)
            : base(name, retType, paren)
        {
            sig.SetParTypes(pars);
            parList = pars;
            parent = paren;
            tabIx = MDTable.Method;
        }

        internal MethodDef(ClassDef paren, MethSig mSig, Param[] pars)
            : base(mSig.name)
        {
            sig = mSig;
            parList = pars;
            parent = paren;
            tabIx = MDTable.Method;
        }

        internal MethodDef(ClassSpec paren, MethSig mSig, Param[] pars)
            : base(mSig.name)
        {
            parent = paren;
            parList = pars;
            sig = mSig;
            tabIx = MDTable.Method;
        }

        internal MethodDef(PEReader buff)
            : base(null)
        {
            rva = buff.ReadUInt32();
            implFlags = buff.ReadUInt16();
            methFlags = buff.ReadUInt16();
            name = buff.GetString();
            sigIx = buff.GetBlobIx();
            parIx = buff.GetIndex(MDTable.Param);
            tabIx = MDTable.Method;
        }

        internal static void Read(PEReader buff, TableRow[] methDefs)
        {
            MethodDef prevDef = null;
            prevDef = new MethodDef(buff);
            methDefs[0] = prevDef;
            for(int i=1; i < methDefs.Length; i++) {
                prevDef.Row = (uint)i;
                MethodDef methDef = new MethodDef(buff);
                prevDef.numPars = methDef.parIx - prevDef.parIx;
                prevDef = methDef;
                methDefs[i] = methDef;
            }
            prevDef.Row = (uint)methDefs.Length;
            prevDef.numPars = (buff.GetTableSize(MDTable.Param)+1) - prevDef.parIx;
        }

        internal static void GetMethodRefs(PEReader buff, uint num, ClassRef parent)
        {
            for(int i=0; i < num; i++) {
                uint rva = buff.ReadUInt32();
                ushort implFlags = buff.ReadUInt16();
                ushort methFlags = buff.ReadUInt16();
                string name = buff.GetString();
                uint sigIx = buff.GetBlobIx();
                uint parIx = buff.GetIndex(MDTable.Param);
                if((methFlags & (ushort)MethAttr.Public) == (ushort)MethAttr.Public) {
                    MethodRef mRef = new MethodRef(parIx, name, sigIx);  // changed
                    mRef.SetParent(parent);
                    //Console.WriteLine(parent.NameString());
                    MethSig mSig = buff.ReadMethSig(mRef, name, sigIx);
                    //mSig.name = name;
                    mRef.SetSig(mSig); // changed
                    parent.AddToMethodList(mRef);
                    //if (parent.GetMethod(mSig) == null) {
                    //  MethodRef mRef = new MethodRef(mSig);
                    //  parent.AddToMethodList(mRef);
                    //}
                }
            }
        }

        /*

            internal static void GetMemberNames(PEReader buff, uint start, uint end, ArrayList membs) {
              buff.SetElementPosition(MDTable.Field,start);
              for (int i=0; i < (end - start); i++, insIx++) {
                uint junk = buff.ReadUInt32();
                junk = buff.ReadUInt16();
                uint flags = buff.ReadUInt16();
                if ((flags & MethAttr.Public) == MethAttr.Public)
                  membs.Add(new MemberIndex(buff,buff.GetString(),buff.GetBlobIx(),false));
                else {
                  junk = buff.GetStringIx();
                  junk = buff.GetBlobIx();
                }
                junk = buff.GetIndex(MDTable.Param);
              }
            }

        */
        /*    internal static MethSig[] GetMethSigs(PEReader buff, int num) {
          MethSig[] meths = new MethSig[num];
          for (int i=0; i < num; i++) {
            uint junk = buff.ReadUInt32();
            junk = buff.ReadUInt16();
            junk = buff.ReadUInt16();
            string mName = new MethSig(buff.GetString());
            meths[i] = buff.ReadMethSig(buff.GetBlobIx());
            junk = buff.GetIndex(MDTable.Param);
          }
        }
        */

        private void DoPars(PEReader buff, bool resolvePars)
        {
            if(sig == null)
                sig = buff.ReadMethSig(this, sigIx);
            sig.name = name;
            parList = new Param[sig.numPars];
            for(uint i=0; i < sig.numPars; i++) {
                parList[i] = (Param)buff.GetElement(MDTable.Param, i+parIx);
                if(resolvePars)
                    parList[i].Resolve(buff, i+parIx, sig.parTypes[i]);
                else
                    parList[i].SetParType(sig.parTypes[i]);
            }
            //parsDone = true;
        }

        private void DoCode(PEReader buff)
        {
            if(rva != 0) {
                if(Diag.DiagOn)
                    Console.WriteLine("Reading byte codes for method " + name);
                buff.ReadByteCodes(this, rva);
            }
        }

        internal sealed override void Resolve(PEReader buff)
        {
            buff.currentMethodScope = this;
            buff.currentClassScope = parent;
            DoPars(buff, true);
            DoCode(buff);
            buff.currentMethodScope = null;
            buff.currentClassScope = null;
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Get the parameters of this method
        /// </summary>
        /// <returns>Array of params of this method</returns>
        public Param[] GetParams()
        {
            //if (!parsDone) DoPars(buffer);
            return parList;
        }

        public void SetParams(Param[] pars)
        {
            parList = pars;
            sig.SetParTypes(pars);
        }

        /// <summary>
        /// Add some attributes to this method descriptor
        /// </summary>
        /// <param name="ma">the attributes to be added</param>
        public void AddMethAttribute(MethAttr ma)
        {
            methFlags |= (ushort)ma;
        }

        /// <summary>
        /// Property to get and set the attributes for this method
        /// </summary>
        public MethAttr GetMethAttributes()
        {
            return (MethAttr)methFlags;
        }
        public void SetMethAttributes(MethAttr ma)
        {
            methFlags = (ushort)ma;
        }

        /// <summary>
        /// Add some implementation attributes to this method descriptor
        /// </summary>
        /// <param name="ia">the attributes to be added</param>
        public void AddImplAttribute(ImplAttr ia)
        {
            implFlags |= (ushort)ia;
        }

        /// <summary>
        /// Property to get and set the implementation attributes for this method
        /// </summary>
        public ImplAttr GetImplAttributes()
        {
            return (ImplAttr)implFlags;
        }
        public void SetImplAttributes(ImplAttr ia)
        {
            implFlags = (ushort)ia;
        }

        public void AddPInvokeInfo(ModuleRef scope, string methName,
          PInvokeAttr callAttr)
        {
            pinvokeImpl = new ImplMap((ushort)callAttr, this, methName, scope);
            methFlags |= PInvokeImpl;
        }

        public void RemovePInvokeInfo()
        {
            pinvokeImpl = null;
            methFlags &= NotPInvoke;
        }

        public void AddSecurity(SecurityAction act, byte[] permissionSet)
        {
            methFlags |= HasSecurity;
            if(security == null)
                security = new ArrayList();
            security.Add(new DeclSecurity(this, act, permissionSet));
        }

        public void AddSecurity(DeclSecurity sec)
        {
            methFlags |= HasSecurity;
            if(security == null)
                security = new ArrayList();
            security.Add(sec);
        }

        public DeclSecurity[] GetSecurity()
        {
            if(security == null)
                return null;
            return (DeclSecurity[])security.ToArray(typeof(DeclSecurity));
        }

        public void RemoveSecurity()
        {
            security = null;
            methFlags &= NoSecurity;
        }

        /// <summary>
        /// Set the maximum stack height for this method
        /// </summary>
        /// <param name="maxStack">the maximum height of the stack</param>
        public void SetMaxStack(int maxStack)
        {
            this.maxStack = maxStack;
        }

        public int GetMaxStack()
        {
            return maxStack;
        }

        /// <summary>
        /// Add local variables to this method
        /// </summary>
        /// <param name="locals">the locals to be added</param>
        /// <param name="initLocals">are locals initialised to default values</param>
        public void AddLocals(Local[] locals, bool initLocals)
        {
            if(locals == null)
                return;
            this.locals = locals;
            this.initLocals = initLocals;
            numLocals = locals.Length;
            for(int i=0; i < numLocals; i++) {
                this.locals[i].SetIndex(i);
            }
        }

        public Local[] GetLocals()
        {
            return locals;
        }

        public void RemoveLocals()
        {
            locals = null;
            numLocals = 0;
            initLocals = false;
        }

        /// <summary>
        /// Mark this method as having an entry point
        /// </summary>
        public void DeclareEntryPoint()
        {
            entryPoint = true;
        }
        public bool HasEntryPoint()
        {
            return entryPoint;
        }
        public void RemoveEntryPoint()
        {
            entryPoint = false;
        }

        /// <summary>
        /// Create a code buffer for this method to add the IL instructions to
        /// </summary>
        /// <returns>a buffer for this method's IL instructions</returns>
        public CILInstructions CreateCodeBuffer()
        {
            code = new CILInstructions(this);
            return code;
        }

        public CILInstructions GetCodeBuffer()
        {
            return code;
        }

        /// <summary>
        /// Make a method reference descriptor for this method to be used 
        /// as a callsite signature for this vararg method
        /// </summary>
        /// <param name="optPars">the optional pars for the vararg method call</param>
        /// <returns></returns>
        public MethodRef MakeVarArgSignature(Type[] optPars)
        {
            MethSig mSig = new MethSig(name);
            mSig.parTypes = sig.parTypes;
            mSig.retType = sig.retType;
            varArgSig = new MethodRef(sig);
            varArgSig.MakeVarArgMethod(this, optPars);
            return varArgSig;
        }

        public MethodRef GetVarArgSignature()
        {
            return varArgSig;
        }

        public MethodRef RefOf()
        {
            return refOf;
        }

        public MethodRef MakeRefOf()
        {
            if(refOf != null)
                return refOf;
            ClassRef parRef = ((ClassDef)parent).MakeRefOf();
            refOf = parRef.GetMethod(name, sig.parTypes);
            if(refOf == null) {
                refOf = new MethodRef(parRef, name, sig.retType, sig.parTypes);
                refOf.defOf = this;
            }
            return refOf;
        }

        /*------------------------- internal functions --------------------------*/

        internal void InsertGenericParam(GenericParam genPar)
        {
            for(int i=0; i < genericParams.Count - genPar.Index; i++) {
                genericParams.Add(null);
            }
            genericParams.Insert((int)genPar.Index, genPar);
        }

        internal override bool isDef()
        {
            return true;
        }

        internal PEFile GetScope()
        {
            return ((ClassDef)parent).GetScope();
        }

        internal void ChangeRefsToDefs(ClassDef newPar, ClassDef[] oldTypes)
        {
            parent = newPar;
            sig.ChangeParTypes(newPar, oldTypes);
            if(code != null)
                code.ChangeRefsToDefs(newPar, oldTypes);
        }

        internal void AddPInvokeInfo(ImplMap impl)
        {
            pinvokeImpl = impl;
            methFlags |= PInvokeImpl;
        }

        internal void AddVarArgSig(MethodRef meth)
        {
            varArgSig = meth;
            meth.MakeVarArgMethod(this, null);
        }

        internal sealed override void TypeSig(MemoryStream sigStream)
        {
            sig.TypeSig(sigStream);
        }

        // fix for Whidbey bug
        internal void AddGenericsToTable(MetaDataOut md)
        {
            for(int i=0; i < genericParams.Count; i++) {
                md.AddToTable(MDTable.GenericParam, (GenericParam)genericParams[i]);
            }
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Method, this);
            nameIx = md.AddToStringsHeap(name);
            for(int i=0; i < genericParams.Count; i++) {
                ((GenericParam)genericParams[i]).BuildMDTables(md);
            }
            if(security != null) {
                for(int i=0; i < security.Count; i++) {
                    ((DeclSecurity)security[i]).BuildMDTables(md);
                }
            }
            if(pinvokeImpl != null)
                pinvokeImpl.BuildMDTables(md);
            if(entryPoint)
                md.SetEntryPoint(this);
            if(locals != null) {
                localSig = new LocalSig(locals);
                localSig.BuildMDTables(md);
            }
            try {
                if(code != null)
                    code.BuildTables(md);
            } catch(InstructionException ex) {
                throw new Exception(ex.AddMethodName(name));
            }
            parIx = md.TableIndex(MDTable.Param);
            for(int i=0; i < sig.numPars; i++) {
                parList[i].seqNo = (ushort)(i+1);
                parList[i].BuildMDTables(md);
            }
            sig.BuildTables(md);
        }

        internal sealed override void BuildSignatures(MetaDataOut md)
        {
            if(locals != null) {
                localSig.BuildSignatures(md);
                locToken = localSig.Token();
            }
            if(code != null) {
                code.CheckCode(locToken, initLocals, maxStack, md);
                textOffset = md.AddCode(code);
                if(Diag.DiagOn)
                    Console.WriteLine("code offset = " + textOffset);
            }
            sig.BuildSignatures(md);
            MemoryStream outSig = new MemoryStream();
            TypeSig(outSig);
            sigIx = md.AddToBlobHeap(outSig.ToArray());
            done = false;
        }

        internal static uint Size(MetaData md)
        {
            return 8 + md.StringsIndexSize() + md.BlobIndexSize() + md.TableIndexSize(MDTable.Param);
        }

        internal sealed override void Write(PEWriter output)
        {
            if(code == null)
                output.Write(0);
            else
                output.WriteCodeRVA(textOffset);
            output.Write(implFlags);
            output.Write(methFlags);
            output.StringsIndex(nameIx);
            output.BlobIndex(sigIx);
            output.WriteIndex(MDTable.Param, parIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 0;
            case (CIx.HasDeclSecurity):
                return 1;
            case (CIx.MemberRefParent):
                return 3;
            case (CIx.MethodDefOrRef):
                return 0;
            case (CIx.MemberForwarded):
                return 1;
            case (CIx.CustomAttributeType):
                return 2;
            case (CIx.TypeOrMethodDef):
                return 1;
            }
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a method defined in another assembly/module
    /// </summary>
    public class MethodRef : Method {
        internal MethodDef defOf;
        MethodDef varArgParent = null;

        /*-------------------- Constructors ---------------------------------*/

        internal MethodRef(Class paren, string name, Type retType, Type[] pars)
            : base(name, retType, paren)
        {
            sig.parTypes = pars;
            if(pars != null)
                sig.numPars = (uint)pars.Length;

        }

        internal MethodRef(uint parIx, string name, uint sigIx)
            : base(name)
        {
            this.parentIx = parIx;
            this.sigIx = sigIx;
        }

        internal MethodRef(MethSig sig)
            : base(sig.name)
        {
            this.sig = sig;
        }

        internal override void Resolve(PEReader buff)
        {
            if(sig == null) {
                buff.currentMethodScope = this;
                buff.currentClassScope = parent;
                sig = buff.ReadMethSig(this, name, sigIx);
                buff.currentMethodScope = null;
                buff.currentClassScope = null;
            }
        }

        internal override Member ResolveParent(PEReader buff)
        {
            if(parent != null)
                return this;
            buff.currentMethodScope = this;
            MetaDataElement paren = buff.GetCodedElement(CIx.MemberRefParent, parentIx);
            buff.currentMethodScope = null;
            if(paren is MethodDef) {
                ((MethodDef)paren).AddVarArgSig(this);
                return this;
            } else if(paren is ClassSpec) {
                ((ClassSpec)paren).AddMethod(this);
                return this;
            } else if(paren is PrimitiveType) {
                paren = MSCorLib.mscorlib.GetDefaultClass();
            } else if(paren is ClassDef) {
                this.sig = buff.ReadMethSig(this, name, sigIx);
                return ((ClassDef)paren).GetMethod(this.sig);
            } else if(paren is TypeSpec) {
                paren = new ConstructedTypeSpec((TypeSpec)paren);
                //Console.WriteLine("Got TypeSpec as parent of Member");
                //return this;
                //throw new Exception("Got TypeSpec as parent of Member");
                //((TypeSpec)paren).AddMethod(buff,this);
            }
            if(paren is ReferenceScope)
                parent = ((ReferenceScope)paren).GetDefaultClass();
            parent = (Class)paren;
            //if ((MethodRef)parent.GetMethodDesc(name) != null) throw new PEFileException("Existing method!!");
            //sig = buff.ReadMethSig(this,name,sigIx);
            //MethodRef existing = (MethodRef)parent.GetMethod(sig);
            //if (existing != null) 
            //  return existing;
            parent.AddToMethodList(this);
            return this;
        }

        public void MakeVarArgMethod(MethodDef paren, Type[] optPars)
        {
            if(paren != null) {
                parent = null;
                varArgParent = paren;
            }
            sig.optParTypes = optPars;
            if(sig.optParTypes != null)
                sig.numOptPars = (uint)sig.optParTypes.Length;
            sig.callConv = CallConv.Vararg;
        }

        internal void MakeGenericPars(uint num)
        {
            for(int i=genericParams.Count; i < num; i++) {
                genericParams.Add(new GenericParam("GPar"+i, this, i));
            }
            //sig.numGenPars = (uint)genericParams.Count;      
        }

        /*------------------------- public set and get methods --------------------------*/

        public void SetParTypes(Type[] pars)
        {
            if(pars == null) {
                sig.numPars = 0;
                return;
            }
            sig.parTypes = pars;
            sig.numPars = (uint)pars.Length;
        }

        public void SetOptParTypes(Type[] pars)
        {
            if(pars == null) {
                sig.numOptPars = 0;
                return;
            }
            sig.optParTypes = pars;
            sig.numOptPars = (uint)sig.optParTypes.Length;
        }

        /*------------------------- internal functions --------------------------*/

        internal sealed override void TypeSig(MemoryStream sigStream)
        {
            sig.TypeSig(sigStream);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.MemberRef, this);
            nameIx = md.AddToStringsHeap(name);
            if(parent != null) {
                if(parent is ClassSpec)
                    md.AddToTable(MDTable.TypeSpec, parent);
                parent.BuildMDTables(md);
            }
            sig.BuildTables(md);
        }

        internal sealed override void BuildSignatures(MetaDataOut md)
        {
            sig.BuildSignatures(md);
            MemoryStream sigStream = new MemoryStream();
            TypeSig(sigStream);
            sigIx = md.AddToBlobHeap(sigStream.ToArray());
            done = false;
        }

        internal static uint Size(MetaData md)
        {
            return md.CodedIndexSize(CIx.MemberRefParent) + md.StringsIndexSize() + md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            if(varArgParent != null)
                output.WriteCodedIndex(CIx.MemberRefParent, varArgParent);
            else
                output.WriteCodedIndex(CIx.MemberRefParent, parent);
            output.StringsIndex(nameIx);
            output.BlobIndex(sigIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 6;
            case (CIx.MethodDefOrRef):
                return 1;
            case (CIx.CustomAttributeType):
                return 3;
            }
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for an overriding method (.override)
    /// </summary>
    public class MethodImpl : MetaDataElement {
        ClassDef parent;
        Method header, body;
        uint classIx = 0, methBodyIx = 0, methDeclIx = 0;
        bool resolved = true;

        /*-------------------- Constructors ---------------------------------*/

        internal MethodImpl(ClassDef par, Method decl, Method bod)
        {
            parent = par;
            header = decl;
            body = bod;
            tabIx = MDTable.MethodImpl;
        }

        internal MethodImpl(PEReader buff, ClassDef par, uint bIx, uint dIx)
        {
            buffer = buff;
            parent = par;
            methBodyIx = bIx;
            methDeclIx = dIx;
            tabIx = MDTable.MethodImpl;
            resolved = false;
        }

        internal MethodImpl(PEReader buff)
        {
            classIx = buff.GetIndex(MDTable.TypeDef);
            methBodyIx = buff.GetCodedIndex(CIx.MethodDefOrRef);
            methDeclIx = buff.GetCodedIndex(CIx.MethodDefOrRef);
            tabIx = MDTable.MethodImpl;
        }

        /*internal static MethodImpl[] GetMethodImpls(PEReader buff, ClassDef paren, uint classIx) {
          buff.SetElementPosition(MDTable.MethodImpl,0);
          ArrayList impls = new ArrayList();
          for (int i=0; i < buff.GetTableSize(MDTable.MethodImpl); i++) {
            uint cIx = buff.GetIndex(MDTable.TypeDef);
            uint bIx = buff.GetCodedIndex(CIx.MethodDefOrRef);
            uint dIx = buff.GetCodedIndex(CIx.MethodDefOrRef);
            if (cIx == classIx) 
              paren.AddMethodOverride(new MethodImpl(buff,paren,bIx,dIx));
          }
          return (MethodImpl[])impls.ToArray(typeof(MethodImpl));
        }
        */

        public Method Body
        {
            get
            {
                if((body == null) && (methBodyIx != 0))
                    body = (Method)buffer.GetCodedElement(CIx.MethodDefOrRef, methBodyIx);
                return body;
            }
            set
            {
                body = value;
                if((!resolved) && (header != null))
                    resolved = true;
            }
        }

        public Method Header
        {
            get
            {
                if((header == null) && (methDeclIx != 0))
                    header = (Method)buffer.GetCodedElement(CIx.MethodDefOrRef, methDeclIx);
                return header;
            }
            set
            {
                header = value;
                if((!resolved) && (body != null))
                    resolved = true;
            }
        }

        internal void SetOwner(ClassDef cl)
        {
            parent = cl;
        }

        internal static void Read(PEReader buff, TableRow[] impls)
        {
            for(int i=0; i < impls.Length; i++)
                impls[i] = new MethodImpl(buff);
        }

        internal override void Resolve(PEReader buff)
        {
            body = (Method)buff.GetCodedElement(CIx.MethodDefOrRef, methBodyIx);
            header = (Method)buff.GetCodedElement(CIx.MethodDefOrRef, methDeclIx);
            parent = (ClassDef)buff.GetElement(MDTable.TypeDef, classIx);
            parent.AddMethodImpl(this);
            resolved = true;
        }

        internal void ResolveMethDetails()
        {
            body = (Method)buffer.GetCodedElement(CIx.MethodDefOrRef, methBodyIx);
            header = (Method)buffer.GetCodedElement(CIx.MethodDefOrRef, methDeclIx);
            resolved = true;
        }

        internal void ChangeRefsToDefs(ClassDef newType, ClassDef[] oldTypes)
        {
            throw new NotYetImplementedException("Merge for MethodImpls");
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.MethodImpl, this);
            if(!resolved)
                ResolveMethDetails();
            if(body is MethodRef)
                body.BuildMDTables(md);
            if(header is MethodRef)
                header.BuildMDTables(md);
        }

        internal static uint Size(MetaData md)
        {
            return md.TableIndexSize(MDTable.TypeDef) + 2 * md.CodedIndexSize(CIx.MethodDefOrRef);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteIndex(MDTable.TypeDef, parent.Row);
            output.WriteCodedIndex(CIx.MethodDefOrRef, body);
            output.WriteCodedIndex(CIx.MethodDefOrRef, header);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for Property and Event methods
    /// </summary>
    public class MethodSemantics : MetaDataElement {
        MethodType type;
        MethodDef meth;
        Feature eventOrProp;
        uint methIx = 0, assocIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal MethodSemantics(MethodType mType, MethodDef method, Feature feature)
        {
            type = mType;
            meth = method;
            eventOrProp = feature;
            sortTable = true;
            tabIx = MDTable.MethodSemantics;
        }

        internal MethodSemantics(PEReader buff)
        {
            type = (MethodType)buff.ReadUInt16();
            methIx = buff.GetIndex(MDTable.Method);
            assocIx = buff.GetCodedIndex(CIx.HasSemantics);
            sortTable = true;
            tabIx = MDTable.MethodSemantics;
        }

        internal static void Read(PEReader buff, TableRow[] methSems)
        {
            for(int i=0; i < methSems.Length; i++)
                methSems[i] = new MethodSemantics(buff);
        }

        internal override void Resolve(PEReader buff)
        {
            meth = (MethodDef)buff.GetElement(MDTable.Method, methIx);
            eventOrProp = (Feature)buff.GetCodedElement(CIx.HasSemantics, assocIx);
            eventOrProp.AddMethod(this);
        }

        internal MethodType GetMethodType()
        {
            return type;
        }

        internal MethodDef GetMethod()
        {
            return meth;
        }

        internal override uint SortKey()
        {
            return meth.Row;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.MethodSemantics, this);
        }

        internal static uint Size(MetaData md)
        {
            return 2 + md.TableIndexSize(MDTable.Method) + md.CodedIndexSize(CIx.HasSemantics);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write((ushort)type);
            output.WriteIndex(MDTable.Method, meth.Row);
            output.WriteCodedIndex(CIx.HasSemantics, eventOrProp);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for a parameter of a method defined in this assembly/module
    /// </summary>
    public class Param : MetaDataElement {
        private static readonly ushort hasDefault = 0x1000;
        private static readonly ushort noDefault = 0xEFFF;
        private static readonly ushort hasFieldMarshal = 0x2000;
        private static readonly ushort noFieldMarshal = 0xDFFF;

        protected string pName;
        protected uint nameIx = 0;
        Type pType;
        internal ushort seqNo = 0;
        ushort parMode;
        Constant defaultVal;
        NativeType marshalType;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new parameter for a method
        /// </summary>
        /// <param name="mode">param mode (in, out, opt)</param>
        /// <param name="parName">parameter name</param>
        /// <param name="parType">parameter type</param>
        public Param(ParamAttr mode, string parName, Type parType)
        {
            pName = parName;
            pType = parType;
            parMode = (ushort)mode;
            tabIx = MDTable.Param;
        }

        internal Param(PEReader buff)
        {
            parMode = buff.ReadUInt16();
            seqNo = buff.ReadUInt16();
            pName = buff.GetString();
            tabIx = MDTable.Param;
        }

        internal static void Read(PEReader buff, TableRow[] pars)
        {
            for(int i=0; i < pars.Length; i++)
                pars[i] = new Param(buff);
        }

        internal void Resolve(PEReader buff, uint fIx, Type type)
        {
            this.pType = type;
            /*      marshalType = FieldMarshal.FindMarshalType(buff,this,
                    buff.MakeCodedIndex(CIx.HasFieldMarshal,MDTable.Param,fIx));
                  defaultVal = ConstantElem.FindConst(buff,this,
                    buff.MakeCodedIndex(CIx.HasConstant,MDTable.Param,fIx));
                    */
        }

        /// <summary>
        /// Add a default value to this parameter
        /// </summary>
        /// <param name="cVal">the default value for the parameter</param>
        public void AddDefaultValue(Constant cVal)
        {
            defaultVal = cVal;
            parMode |= hasDefault;
        }

        /// <summary>
        /// Get the default constant value for this parameter
        /// </summary>
        /// <returns></returns>
        public Constant GetDefaultValue()
        {
            return defaultVal;
        }

        /// <summary>
        /// Remove the default constant value for this parameter
        /// </summary>
        public void RemoveDefaultValue()
        {
            defaultVal = null;
            parMode &= noDefault;
        }

        /// <summary>
        /// Add marshalling information about this parameter
        /// </summary>
        public void SetMarshalType(NativeType mType)
        {
            marshalType = mType;
            parMode |= hasFieldMarshal;
        }
        /// <summary>
        /// Get the parameter marshalling information
        /// </summary>
        /// <returns>The native type to marshall to</returns>
        public NativeType GetMarshalType()
        {
            return marshalType;
        }

        /// <summary>
        /// Remove any marshalling information for this parameter
        /// </summary>
        public void RemoveMashalType()
        {
            marshalType = null;
            parMode &= noFieldMarshal;
        }

        /// <summary>
        /// Get the type of this parameter
        /// </summary>
        public Type GetParType()
        {
            return pType;
        }

        /// <summary>
        /// Set the type of this parameter
        /// </summary>
        public void SetParType(Type parType)
        {
            pType = parType;
        }

        public void AddAttribute(ParamAttr att)
        {
            this.parMode |= (ushort)att;
        }

        public ParamAttr GetAttributes()
        {
            return (ParamAttr)parMode;
        }

        public void SetAttributes(ParamAttr att)
        {
            this.parMode = (ushort)att;
        }

        public string GetName()
        {
            return pName;
        }
        public void SetName(string nam)
        {
            pName = nam;
        }

        /*------------------------ internal functions ----------------------------*/

        internal Param Copy(Type paramType)
        {
            return new Param((ParamAttr)parMode, pName, paramType);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Param, this);
            nameIx = md.AddToStringsHeap(pName);
            if(defaultVal != null) {
                ConstantElem constElem = new ConstantElem(this, defaultVal);
                constElem.BuildMDTables(md);
            }
            if(marshalType != null) {
                FieldMarshal marshalInfo = new FieldMarshal(this, marshalType);
                marshalInfo.BuildMDTables(md);
            }
        }

        internal void TypeSig(MemoryStream str)
        {
            pType.TypeSig(str);
        }

        internal static uint Size(MetaData md)
        {
            return 4 + md.StringsIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(parMode);
            output.Write(seqNo);
            output.StringsIndex(nameIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 4;
            case (CIx.HasConstant):
                return 1;
            case (CIx.HasFieldMarshal):
                return 1;
            }
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Base class for scopes (extended by Module, ModuleRef, Assembly, AssemblyRef)
    /// </summary>
    public abstract class ResolutionScope : MetaDataElement {
        internal protected uint nameIx = 0;
        internal protected string name;
        internal protected ArrayList classes = new ArrayList();
        internal protected bool readAsDef = false;

        /*-------------------- Constructors ---------------------------------*/

        internal ResolutionScope(string name)
        {
            this.name = name;
        }

        internal virtual void AddToClassList(Class aClass)
        {
            classes.Add(aClass);
        }

        internal Class GetExistingClass(string nameSpace, string name)
        {
            for(int i=0; i < classes.Count; i++) {
                Class aClass = (Class)classes[i];
                if((aClass.Name() == name) && (aClass.NameSpace() == nameSpace))
                    return aClass;
            }
            return null;
        }

        protected Class GetClass(string nameSpace, string name, bool both)
        {
            for(int i=0; i < classes.Count; i++) {
                Object aClass = classes[i];
                if((((Class)aClass).Name() == name) && 
          (!both || (both && (((Class)aClass).NameSpace() == nameSpace))))
                    return (Class)aClass;
            }
            return null;
        }

        /// <summary>
        /// Delete a class from this module
        /// </summary>
        /// <param name="aClass">The name of the class to be deleted</param>
        public void RemoveClass(Class aClass)
        {
            classes.Remove(aClass);
        }

        /// <summary>
        /// Delete the class at an index in the class array
        /// </summary>
        /// <param name="ix">The index of the class to be deleted (from 0)</param>
        public void RemoveClass(int ix)
        {
            classes.RemoveAt(ix);
        }

        public string Name()
        {
            return name;
        }

        internal override string NameString()
        {
            return "[" + name + "]";
        }

    }

    /**************************************************************************/
    /// <summary>
    /// A scope for definitions
    /// </summary>
    public abstract class DefiningScope : ResolutionScope {

        /*-------------------- Constructors ---------------------------------*/

        internal DefiningScope(string name)
            : base(name)
        {
            readAsDef = true;
        }

        internal override void AddToClassList(Class aClass)
        {
            ((ClassDef)aClass).SetScope((PEFile)this);
            classes.Add(aClass);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for an assembly (.assembly)
    /// </summary>
    public class Assembly : DefiningScope {
        //internal static Hashtable Assemblies = new Hashtable();
        ushort majorVer, minorVer, buildNo, revisionNo;
        uint flags;
        HashAlgorithmType hashAlgId = HashAlgorithmType.None;
        uint keyIx = 0, cultIx = 0;
        byte[] publicKey;
        string culture;
        internal AssemblyRef refOf;
        ArrayList security;
        internal PEFile pefile;

        /*-------------------- Constructors ---------------------------------*/

        internal Assembly(string name, PEFile pefile)
            : base(name)
        {
            this.pefile = pefile;
            tabIx = MDTable.Assembly;
        }

        internal Assembly(string name, HashAlgorithmType hashAlgId, ushort majVer,
          ushort minVer, ushort bldNo, ushort revNo, uint flags, byte[] pKey,
          string cult, PEFile pefile)
            : base(name)
        {
            this.hashAlgId = hashAlgId;
            this.majorVer = majVer;
            this.minorVer = minVer;
            this.buildNo = bldNo;
            this.revisionNo = revNo;
            this.flags = flags;
            this.publicKey = pKey;
            this.culture = cult;
            tabIx = MDTable.Assembly;
        }

        internal static AssemblyRef ReadAssemblyRef(PEReader buff)
        {
            buff.SetElementPosition(MDTable.Assembly, 1);
            HashAlgorithmType hAlg = (HashAlgorithmType)buff.ReadUInt32();
            ushort majVer = buff.ReadUInt16();
            ushort minVer = buff.ReadUInt16();
            ushort bldNo = buff.ReadUInt16();
            ushort revNo = buff.ReadUInt16();
            uint flags = buff.ReadUInt32();
            byte[] pKey =  buff.GetBlob();
            string name = buff.GetString();
            string cult = buff.GetString();
            AssemblyRef assemRef = null;
            if(name.ToLower() == "mscorlib") {
                assemRef = MSCorLib.mscorlib;
                assemRef.AddVersionInfo(majVer, minVer, bldNo, revNo);
                if(pKey.Length > 8)
                    assemRef.AddKey(pKey);
                else
                    assemRef.AddKeyToken(pKey);
                assemRef.AddCulture(cult);
                assemRef.SetFlags(flags);
            } else {
                assemRef = new AssemblyRef(name, majVer, minVer, bldNo, revNo, flags, pKey, cult, null);
            }
            //AssemblyRef assemRef = new AssemblyRef(name,majVer,minVer,bldNo,revNo,flags,pKey,cult,null);
            assemRef.ReadAsDef();
            return assemRef;
        }

        internal static void Read(PEReader buff, TableRow[] table, PEFile pefile)
        {
            for(int i=0; i < table.Length; i++) {
                HashAlgorithmType hAlg = (HashAlgorithmType)buff.ReadUInt32();
                ushort majVer = buff.ReadUInt16();
                ushort minVer = buff.ReadUInt16();
                ushort bldNo = buff.ReadUInt16();
                ushort revNo = buff.ReadUInt16();
                uint flags = buff.ReadUInt32();
                byte[] pKey =  buff.GetBlob();
                string name = buff.GetString();
                string cult = buff.GetString();
                table[i] = new Assembly(name, hAlg, majVer, minVer, bldNo, revNo, flags, pKey, cult, pefile);
            }
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Add details about an assembly
        /// </summary>
        /// <param name="majVer">Major Version</param>
        /// <param name="minVer">Minor Version</param>
        /// <param name="bldNo">Build Number</param>
        /// <param name="revNo">Revision Number</param>
        /// <param name="key">Hash Key</param>
        /// <param name="hash">Hash Algorithm</param>
        /// <param name="cult">Culture</param>
        public void AddAssemblyInfo(int majVer, int minVer, int bldNo, int revNo,
          byte[] key, HashAlgorithmType hash, string cult)
        {
            majorVer = (ushort)majVer;
            minorVer = (ushort)minVer;
            buildNo = (ushort)bldNo;
            revisionNo = (ushort)revNo;
            hashAlgId = hash;
            publicKey = key;
            culture = cult;
        }

        /// <summary>
        /// Get the major version number for this Assembly
        /// </summary>
        /// <returns>major version number</returns>
        public int MajorVersion()
        {
            return majorVer;
        }
        /// <summary>
        /// Get the minor version number for this Assembly
        /// </summary>
        /// <returns>minor version number</returns>
        public int MinorVersion()
        {
            return minorVer;
        }
        /// <summary>
        /// Get the build number for this Assembly
        /// </summary>
        /// <returns>build number</returns>
        public int BuildNumber()
        {
            return buildNo;
        }
        /// <summary>
        /// Get the revision number for this Assembly
        /// </summary>
        /// <returns>revision number</returns>
        public int RevisionNumber()
        {
            return revisionNo;
        }
        /// <summary>
        /// Get the public key for this Assembly
        /// </summary>
        /// <returns>public key bytes</returns>
        public byte[] Key()
        {
            return publicKey;
        }
        /// <summary>
        /// Get the type of the hash algorithm for this Assembly
        /// </summary>
        /// <returns>hash algorithm type</returns>
        public HashAlgorithmType HashAlgorithm()
        {
            return hashAlgId;
        }
        /// <summary>
        /// Get the culture information for this Assembly
        /// </summary>
        /// <returns>culture string</returns>
        public string Culture()
        {
            return culture;
        }

        /// <summary>
        /// Add some security action(s) to this Assembly
        /// </summary>
        public void AddSecurity(SecurityAction act, byte[] permissionSet)
        {
            AddSecurity(new DeclSecurity(this, act, permissionSet));
            // securityActions = permissionSet;
        }

        /// <summary>
        /// Get the security information for this assembly
        /// </summary>
        /// <returns>security information</returns>
        public DeclSecurity[] GetSecurity()
        {
            if(security == null)
                return null;
            return (DeclSecurity[])security.ToArray(typeof(DeclSecurity));
        }

        /// <summary>
        /// Check if this assembly has security information
        /// </summary>
        public bool HasSecurity()
        {
            return security != null;
        }

        /// <summary>
        /// Set the attributes for this assembly
        /// </summary>
        /// <param name="aa">assembly attribute</param>     
        public void SetAssemblyAttr(AssemAttr aa)
        {
            flags = (uint)aa;
        }

        /// <summary>
        /// Add an attribute for this assembly
        /// </summary>
        /// <param name="aa">assembly attribute</param>     
        public void AddAssemblyAttr(AssemAttr aa)
        {
            flags |= (uint)aa;
        }

        /// <summary>
        /// Get the attributes of this assembly
        /// </summary>
        /// <returns>assembly attributes</returns>
        public AssemAttr AssemblyAttributes()
        {
            return (AssemAttr)flags;
        }

        /// <summary>
        /// Make an AssemblyRef descriptor for this Assembly
        /// </summary>
        /// <returns>AssemblyRef descriptor for this Assembly</returns>
        public AssemblyRef MakeRefOf()
        {
            if(refOf == null) {
                refOf = new AssemblyRef(name, majorVer, minorVer, buildNo, revisionNo,
                  flags, publicKey, culture, null);
            }
            return refOf;
        }

        /*------------------------ internal functions ----------------------------*/

        internal void AddSecurity(DeclSecurity sec)
        {
            if(security == null)
                security = new ArrayList();
            security.Add(sec);
        }

        internal static uint Size(MetaData md)
        {
            return 16 + md.BlobIndexSize() + 2 * md.StringsIndexSize();
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Assembly, this);
            nameIx = md.AddToStringsHeap(name);
            cultIx = md.AddToStringsHeap(culture);
            keyIx = md.AddToBlobHeap(publicKey);
            if(security != null) {
                for(int i=0; i < security.Count; i++) {
                    ((DeclSecurity)security[i]).BuildMDTables(md);
                }
            }
        }

        internal sealed override void Write(PEWriter output)
        {
            //Console.WriteLine("Writing assembly element with nameIx of " + nameIx + " at file offset " + output.Seek(0,SeekOrigin.Current));
            output.Write((uint)hashAlgId);
            output.Write(majorVer);
            output.Write(minorVer);
            output.Write(buildNo);
            output.Write(revisionNo);
            output.Write(flags);
            output.BlobIndex(keyIx);
            output.StringsIndex(nameIx);
            output.StringsIndex(cultIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 14;
            case (CIx.HasDeclSecurity):
                return 2;
            }
            return 0;
        }


    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a module
    /// </summary>
    public abstract class Module : DefiningScope {
        Guid mvid;
        uint mvidIx = 0;
        internal ModuleRef refOf;
        /// <summary>
        /// The default class "Module" for globals
        /// </summary>
        protected ClassDef defaultClass;
        /// <summary>
        /// Is this module a .dll or .exe
        /// </summary>
        //protected bool isDLL;
        /// <summary>
        /// Is this module mscorlib.dll
        /// </summary>
        protected bool ismscorlib = false;
        /// <summary>
        /// Managed resources for this module
        /// </summary>
        protected ArrayList resources = new ArrayList();

        /*-------------------- Constructors ---------------------------------*/

        internal Module(string mName)
            : base(GetBaseName(mName))
        {
            mvid = Guid.NewGuid();
            //isDLL = name.EndsWith(".dll") || name.EndsWith(".DLL");
            defaultClass = new ClassDef((PEFile)this, TypeAttr.Private, "", "<Module>");
            defaultClass.MakeSpecial();
            tabIx = MDTable.Module;
            ismscorlib = name.ToLower() == "mscorlib.dll";
            if(Diag.DiagOn)
                Console.WriteLine("Module name = " + name);
        }

        internal void Read(PEReader buff)
        {
            buff.ReadZeros(2);
            name = buff.GetString();
            mvid = buff.GetGUID();
            uint junk = buff.GetGUIDIx();
            junk = buff.GetGUIDIx();
            if(Diag.DiagOn)
                Console.WriteLine("Reading module with name " + name + " and Mvid = " + mvid);
            ismscorlib = name.ToLower() == "mscorlib.dll";
        }

        internal static ModuleRef ReadModuleRef(PEReader buff)
        {
            buff.ReadZeros(2);
            string name = buff.GetString();
            uint junk = buff.GetGUIDIx();
            junk = buff.GetGUIDIx();
            junk = buff.GetGUIDIx();
            ModuleRef mRef = new ModuleRef(new ModuleFile(name, null));
            mRef.ReadAsDef();
            return mRef;
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Add a class to this Module
        /// If this class already exists, throw an exception
        /// </summary>
        /// <param name="attrSet">attributes of this class</param>
        /// <param name="nsName">name space name</param>
        /// <param name="name">class name</param>
        /// <returns>a descriptor for this new class</returns>
        public ClassDef AddClass(TypeAttr attrSet, string nsName, string name)
        {
            ClassDef aClass = GetClass(nsName, name);
            if(aClass != null)
                throw new DescriptorException("Class " + aClass.NameString());
            aClass = new ClassDef((PEFile)this, attrSet, nsName, name);
            classes.Add(aClass);
            return aClass;
        }

        /// <summary>
        /// Add a class which extends System.ValueType to this Module
        /// If this class already exists, throw an exception
        /// </summary>
        /// <param name="attrSet">attributes of this class</param>
        /// <param name="nsName">name space name</param>
        /// <param name="name">class name</param>
        /// <returns>a descriptor for this new class</returns>
        public ClassDef AddValueClass(TypeAttr attrSet, string nsName, string name)
        {
            ClassDef aClass = AddClass(attrSet, nsName, name);
            aClass.SuperType = MSCorLib.mscorlib.ValueType();
            aClass.MakeValueClass();
            return aClass;
        }

        /// <summary>
        /// Add a class to this PE File
        /// </summary>
        /// <param name="attrSet">attributes of this class</param>
        /// <param name="nsName">name space name</param>
        /// <param name="name">class name</param>
        /// <param name="superType">super type of this class (extends)</param>
        /// <returns>a descriptor for this new class</returns>
        public ClassDef AddClass(TypeAttr attrSet, string nsName, string name, Class superType)
        {
            ClassDef aClass = AddClass(attrSet, nsName, name);
            aClass.SuperType = superType;
            return aClass;
        }

        /// <summary>
        /// Add a class to this module
        /// If this class already exists, throw an exception
        /// </summary>
        /// <param name="aClass">The class to be added</param>
        public void AddClass(ClassDef aClass)
        {
            ClassDef eClass = GetClass(aClass.NameSpace(), aClass.Name());
            if(eClass != null)
                throw new DescriptorException("Class " + aClass.NameString());
            classes.Add(aClass);
            // MERGE change Refs to Defs here, fix this
            aClass.SetScope((PEFile)this);
        }

        /// <summary>
        /// Get a class of this module, if no class exists, return null
        /// </summary>
        /// <param name="name">The name of the class to get</param>
        /// <returns>ClassDef for name or null</returns>
        public ClassDef GetClass(string name)
        {
            return (ClassDef)GetClass(null, name, false);
        }

        /// <summary>
        /// Get a class of this module, if no class exists, return null
        /// </summary>
        /// <param name="nsName">The namespace of the class</param>
        /// <param name="name">The name of the class to get</param>
        /// <returns>ClassDef for nsName.name or null</returns>
        public ClassDef GetClass(string nsName, string name)
        {
            return (ClassDef)GetClass(nsName, name, true);
        }

        /// <summary>
        /// Get all the classes of this module
        /// </summary>
        /// <returns>An array containing a ClassDef for each class of this module</returns>
        public ClassDef[] GetClasses()
        {
            return (ClassDef[])classes.ToArray(typeof(ClassDef));
        }

        /// <summary>
        /// Add a "global" method to this module
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">method parameters</param>
        /// <returns>a descriptor for this new "global" method</returns>
        public MethodDef AddMethod(string name, Type retType, Param[] pars)
        {
            MethodDef newMeth = defaultClass.AddMethod(name, retType, pars);
            return newMeth;
        }

        /// <summary>
        /// Add a "global" method to this module
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="genPars">generic parameters</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">method parameters</param>
        /// <returns>a descriptor for this new "global" method</returns>
        public MethodDef AddMethod(string name, GenericParam[] genPars, Type retType, Param[] pars)
        {
            MethodDef newMeth = defaultClass.AddMethod(name, genPars, retType, pars);
            return newMeth;
        }

        /// <summary>
        /// Add a "global" method to this module
        /// </summary>
        /// <param name="mAtts">method attributes</param>
        /// <param name="iAtts">method implementation attributes</param>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">method parameters</param>
        /// <returns>a descriptor for this new "global" method</returns>
        public MethodDef AddMethod(MethAttr mAtts, ImplAttr iAtts, string name, Type retType, Param[] pars)
        {
            MethodDef newMeth = defaultClass.AddMethod(mAtts, iAtts, name, retType, pars);
            return newMeth;
        }

        /// <summary>
        /// Add a "global" method to this module
        /// </summary>
        /// <param name="mAtts">method attributes</param>
        /// <param name="iAtts">method implementation attributes</param>
        /// <param name="name">method name</param>
        /// <param name="genPars">generic parameters</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">method parameters</param>
        /// <returns>a descriptor for this new "global" method</returns>
        public MethodDef AddMethod(MethAttr mAtts, ImplAttr iAtts, string name, GenericParam[] genPars, Type retType, Param[] pars)
        {
            MethodDef newMeth = defaultClass.AddMethod(mAtts, iAtts, name, genPars, retType, pars);
            return newMeth;
        }

        /// <summary>
        /// Add a "global" method to this module
        /// </summary>
        /// <param name="meth">The method to be added</param>
        public void AddMethod(MethodDef meth)
        {
            defaultClass.AddMethod(meth);
        }

        /// <summary>
        /// Get a method of this module, if it exists
        /// </summary>
        /// <param name="name">The name of the method to get</param>
        /// <returns>MethodDef for name, or null if one does not exist</returns>
        public MethodDef GetMethod(string name)
        {
            return defaultClass.GetMethod(name);
        }

        /// <summary>
        /// Get all the methods of this module with a specified name
        /// </summary>
        /// <param name="name">The name of the method(s)</param>
        /// <returns>An array of all the methods of this module called "name" </returns>
        public MethodDef[] GetMethods(string name)
        {
            return defaultClass.GetMethods(name);
        }

        /// <summary>
        /// Get a method of this module, if it exists
        /// </summary>
        /// <param name="name">The name of the method to get</param>
        /// <param name="parTypes">The signature of the method</param>
        /// <returns>MethodDef for name(parTypes), or null if one does not exist</returns>
        public MethodDef GetMethod(string name, Type[] parTypes)
        {
            return defaultClass.GetMethod(name, parTypes);
        }

        /// <summary>
        /// Get all the methods of this module
        /// </summary>
        /// <returns>An array of all the methods of this module</returns>
        public MethodDef[] GetMethods()
        {
            return defaultClass.GetMethods();
        }

        /// <summary>
        /// Delete a method from this module
        /// </summary>
        /// <param name="meth">The method to be deleted</param>
        public void RemoveMethod(MethodDef meth)
        {
            defaultClass.RemoveMethod(meth);
        }

        /// <summary>
        /// Delete a method from this module
        /// </summary>
        /// <param name="name">The name of the method to be deleted</param>
        public void RemoveMethod(string name)
        {
            defaultClass.RemoveMethod(name);
        }

        /// <summary>
        /// Delete a method from this module
        /// </summary>
        /// <param name="name">The name of the method to be deleted</param>
        /// <param name="parTypes">The signature of the method to be deleted</param>
        public void RemoveMethod(string name, Type[] parTypes)
        {
            defaultClass.RemoveMethod(name, parTypes);
        }

        /// <summary>
        /// Delete a method from this module
        /// </summary>
        /// <param name="ix">The index of the method (in the method array
        /// returned by GetMethods()) to be deleted</param>
        public void RemoveMethod(int ix)
        {
            defaultClass.RemoveMethod(ix);
        }

        /// <summary>
        /// Add a "global" field to this module
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="fType">field type</param>
        /// <returns>a descriptor for this new "global" field</returns>
        public FieldDef AddField(string name, Type fType)
        {
            FieldDef newField = defaultClass.AddField(name, fType);
            return newField;
        }

        /// <summary>
        /// Add a "global" field to this module
        /// </summary>
        /// <param name="attrSet">attributes of this field</param>
        /// <param name="name">field name</param>
        /// <param name="fType">field type</param>
        /// <returns>a descriptor for this new "global" field</returns>
        public FieldDef AddField(FieldAttr attrSet, string name, Type fType)
        {
            FieldDef newField = defaultClass.AddField(attrSet, name, fType);
            return newField;
        }

        /// <summary>
        /// Add a "global" field to this module
        /// </summary>
        /// <param name="fld">The field to be added</param>
        public void AddField(FieldDef fld)
        {
            defaultClass.AddField(fld);
        }

        /// <summary>
        /// Get a field of this module, if it exists
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <returns>FieldDef for "name", or null if one doesn't exist</returns>
        public FieldDef GetField(string name)
        {
            return defaultClass.GetField(name);
        }

        /// <summary>
        /// Get all the fields of this module
        /// </summary>
        /// <returns>An array of all the fields of this module</returns>
        public FieldDef[] GetFields()
        {
            return defaultClass.GetFields();
        }

        /// <summary>
        /// Make a ModuleRef for this Module.
        /// </summary>
        /// <returns>ModuleRef for this Module</returns>
        public ModuleRef MakeRefOf(/*bool hasEntryPoint, byte[] hashValue*/)
        {
            if(refOf == null) {
                refOf = new ModuleRef(name/*,hasEntryPoint,hashValue*/);
                refOf.defOf = this;
            }/* else {  // fix this
        if (hasEntryPoint)
          refOf.SetEntryPoint();
        refOf.SetHash(hashValue);
      }*/
            return refOf;
        }

        /// <summary>
        /// Set the name for this module
        /// </summary>
        /// <param name="newName">New module name</param>
        public void SetName(string newName)
        {
            name = newName;
            //isDLL = name.EndsWith(".dll") || name.EndsWith(".DLL");
        }

        public void SetMVid(Guid guid)
        {
            mvid = guid;
        }

        public Guid GetMVid()
        {
            return mvid;
        }

        /*------------------------- internal functions --------------------------*/

        internal bool isMSCorLib()
        {
            return ismscorlib;
        }

        internal bool isDefaultClass(ClassDef aClass)
        {
            return aClass == defaultClass;
        }

        private static string GetBaseName(string name)
        {
            // more to this??
            if(name.IndexOf("\\") != -1)
                name = name.Substring(name.LastIndexOf("\\")+1);
            return name;
        }

        internal void SetDefaultClass(ClassDef dClass)
        {
            defaultClass = dClass;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.Module, this);
            nameIx = md.AddToStringsHeap(name);
            mvidIx = md.AddToGUIDHeap(mvid);
            defaultClass.BuildTables(md);
            for(int i=0; i < classes.Count; i++) {
                ((Class)classes[i]).BuildMDTables(md);
            }
            for(int i=0; i < resources.Count; i++) {
                ((ManifestResource)resources[i]).BuildMDTables(md);
            }
        }

        internal static uint Size(MetaData md)
        {
            return 2 + md.StringsIndexSize() + 3 * md.GUIDIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write((short)0);
            output.StringsIndex(nameIx);
            output.GUIDIndex(mvidIx);
            output.GUIDIndex(0);
            output.GUIDIndex(0);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 7;
            case (CIx.ResolutionScope):
                return 0;
            }
            return 0;
        }


    }
    /**************************************************************************/
    /// <summary>
    /// A scope for descriptors which are referenced
    /// </summary>
    public abstract class ReferenceScope : ResolutionScope {
        /// <summary>
        /// A default class decriptor for globals
        /// </summary>
        protected ClassRef defaultClass;

        /*-------------------- Constructors ---------------------------------*/

        internal ReferenceScope(string name)
            : base(name)
        {
            defaultClass = new ClassRef(this, "", "");
            defaultClass.MakeSpecial();
        }

        internal void ReadAsDef()
        {
            readAsDef = true;
        }

        internal ClassRef GetDefaultClass()
        {
            return defaultClass;
        }

        internal void SetDefaultClass(ClassRef dClass)
        {
            defaultClass = dClass;
        }

        internal override void AddToClassList(Class aClass)
        {
            ((ClassRef)aClass).SetScope(this);
            classes.Add(aClass);
        }

        internal void ReplaceClass(Class aClass)
        {
            bool found = false;
            for(int i=0; (i < classes.Count) && !found; i++) {
                if(((Class)classes[i]).Name() == aClass.Name()) {
                    found = true;
                }
            }
            if(!found)
                classes.Add(aClass);
        }

        internal bool isDefaultClass(ClassRef aClass)
        {
            return aClass == defaultClass;
        }

        /// <summary>
        /// Add a class to this Scope.  If this class already exists, throw
        /// an exception
        /// </summary>
        /// <param name="newClass">The class to be added</param>
        public void AddClass(ClassRef newClass)
        {
            ClassRef aClass = (ClassRef)GetClass(newClass.NameSpace(), newClass.Name(), true);
            if(aClass != null)
                throw new DescriptorException("Class " + newClass.NameString());
            if(Diag.DiagOn)
                Console.WriteLine("Adding class " + newClass.Name() + " to ResolutionScope " + name);
            classes.Add(newClass);
            // Change Refs to Defs here
            newClass.SetScope(this);
        }

        /// <summary>
        /// Add a class to this Scope.  If the class already exists,
        /// throw an exception.  
        /// </summary>
        /// <param name="nsName">name space name</param>
        /// <param name="name">class name</param>
        /// <returns>a descriptor for this class in another module</returns>
        public virtual ClassRef AddClass(string nsName, string name)
        {
            ClassRef aClass = GetClass(nsName, name);
            if(aClass != null) {
                if((aClass is SystemClass) && (!((SystemClass)aClass).added))
                    ((SystemClass)aClass).added = true;
                else
                    throw new DescriptorException("Class " + aClass.NameString());
            } else {
                aClass = new ClassRef(this, nsName, name);
                classes.Add(aClass);
            }
            return aClass;
        }

        /// <summary>
        /// Add a value class to this scope.  If the class already exists,
        /// throw an exception.  
        /// </summary>
        /// <param name="nsName">name space name</param>
        /// <param name="name">class name</param>
        /// <returns></returns>
        public virtual ClassRef AddValueClass(string nsName, string name)
        {
            ClassRef aClass = AddClass(nsName, name);
            aClass.MakeValueClass();
            return aClass;
        }

        /// <summary>
        /// Get a class of this scope, if it exists.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <returns>ClassRef for "name".</returns>
        public ClassRef GetClass(string name)
        {
            return (ClassRef)GetClass(null, name, false);
        }

        /// <summary>
        /// Get a class of this scope, if it exists.
        /// </summary>
        /// <param name="nsName">The namespace of the class.</param>
        /// <param name="name">The name of the class.</param>
        /// <returns>ClassRef for "nsName.name".</returns>
        public ClassRef GetClass(string nsName, string name)
        {
            return (ClassRef)GetClass(nsName, name, true);
        }

        /// <summary>
        /// Get all the classes in this scope.
        /// </summary>
        /// <returns>An array of all the classes in this scope.</returns>
        public ClassRef[] GetClasses()
        {
            return (ClassRef[])classes.ToArray(typeof(ClassRef));
        }

        /// <summary>
        /// Fetch a MethodRef descriptor for the method "retType name (pars)".
        /// If one exists, it is returned, else one is created.
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">method parameter types</param>
        /// <returns>a descriptor for this method in anther module</returns>
        public MethodRef AddMethod(string name, Type retType, Type[] pars)
        {
            MethodRef meth = defaultClass.AddMethod(name, retType, pars);
            return meth;
        }

        /// <summary>
        /// Fetch a MethodRef descriptor for the method "retType name (pars, optPars)".
        /// If one exists, it is returned, else one is created.
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameter types</param>
        /// <param name="optPars">optional param types for this vararg method</param>
        /// <returns>a descriptor for this method</returns>
        public MethodRef AddVarArgMethod(string name, Type retType, Type[] pars, Type[] optPars)
        {
            MethodRef meth = defaultClass.AddVarArgMethod(name, retType, pars, optPars);
            return meth;
        }

        /// <summary>
        /// Add a method to this scope.
        /// </summary>
        /// <param name="meth">The method to be added.</param>
        public void AddMethod(MethodRef meth)
        {
            defaultClass.AddMethod(meth);
        }

        //		internal void CheckAddMethod(MethodRef meth) {
        //			defaultClass.CheckAddMethod(meth);
        //		}
        /*
            internal void CheckAddMethods(ArrayList meths) {
              for (int i=0; i < meths.Count; i++) {
                Method meth = (Method)meths[i];
                defaultClass.CheckAddMethod(meth);
                meth.SetParent(this);
              }
            }

            internal MethodRef GetMethod(string name, uint sigIx) {
              return defaultClass.GetMethod(name,sigIx);
            }
            */

        /// <summary>
        /// Get a method of this scope, if it exists.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <returns>MethodRef for "name", or null if none exists.</returns>
        public MethodRef GetMethod(string name)
        {
            return defaultClass.GetMethod(name);
        }

        /// <summary>
        /// Get all the methods with a specified name in this scope.
        /// </summary>
        /// <param name="name">The name of the method(s).</param>
        /// <returns>An array of all the methods called "name".</returns>
        public MethodRef[] GetMethods(string name)
        {
            return defaultClass.GetMethods(name);
        }


        /// <summary>
        /// Get a method of this scope, if it exists.
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="parTypes">The signature of the method.</param>
        /// <returns>MethodRef for name(parTypes).</returns>
        public MethodRef GetMethod(string name, Type[] parTypes)
        {
            return defaultClass.GetMethod(name, parTypes);
        }

        /// <summary>
        /// Get a vararg method of this scope, if it exists.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="parTypes">The signature of the method.</param>
        /// <param name="optPars">The optional parameters of the vararg method.</param>
        /// <returns>MethodRef for name(parTypes,optPars).</returns>
        public MethodRef GetMethod(string name, Type[] parTypes, Type[] optPars)
        {
            return defaultClass.GetMethod(name, parTypes, optPars);
        }

        /// <summary>
        /// Get all the methods in this module
        /// </summary>
        /// <returns>Array of the methods of this module</returns>
        public MethodRef[] GetMethods()
        {
            return defaultClass.GetMethods();
        }

        /// <summary>
        /// Delete a method from this scope.
        /// </summary>
        /// <param name="meth">The method to be deleted.</param>
        public void RemoveMethod(MethodRef meth)
        {
            defaultClass.RemoveMethod(meth);
        }

        /// <summary>
        /// Delete a method from this scope.  If there are multiple methods with
        /// the same name, the first on the list will be deleted.
        /// </summary>
        /// <param name="name">The name of the method to delete.</param>
        public void RemoveMethod(string name)
        {
            defaultClass.RemoveMethod(name);
        }

        /// <summary>
        /// Delete a method from this scope.
        /// </summary>
        /// <param name="name">The name of the method to be deleted.</param>
        /// <param name="parTypes">The signature of the method to be deleted.</param>
        public void RemoveMethod(string name, Type[] parTypes)
        {
            defaultClass.RemoveMethod(name, parTypes);
        }

        /// <summary>
        /// Delete a (vararg) method from this scope.
        /// </summary>
        /// <param name="name">The name of the method to be deleted.</param>
        /// <param name="parTypes">The signature of the method to be deleted.</param>
        /// <param name="optTypes">The optional parameters of the vararg method.</param>
        public void RemoveMethod(string name, Type[] parTypes, Type[] optTypes)
        {
            defaultClass.RemoveMethod(name, parTypes, optTypes);
        }

        /// <summary>
        /// Delete a method from this scope.
        /// </summary>
        /// <param name="index">The index of the method to be deleted.  Index
        /// into array returned by GetMethods().</param>
        public void RemoveMethod(int index)
        {
            defaultClass.RemoveMethod(index);
        }

        /// <summary>
        /// Add a field to this scope.
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="fType">field type</param>
        /// <returns>a descriptor for the field "name" in this scope</returns>
        public FieldRef AddField(string name, Type fType)
        {
            FieldRef field = defaultClass.AddField(name, fType);
            return field;
        }

        /// <summary>
        /// Add a field to this scope.
        /// </summary>
        /// <param name="fld">The field to be added</param>
        public void AddField(FieldRef fld)
        {
            defaultClass.AddField(fld);
        }

        /// <summary>
        /// Add a number of fields to this scope.
        /// </summary>
        /// <param name="flds">The fields to be added.</param>
        internal void AddFields(ArrayList flds)
        {
            for(int i=0; i < flds.Count; i++) {
                FieldRef fld = (FieldRef)flds[i];
                defaultClass.AddField(fld);
            }
        }

        /// <summary>
        /// Fetch the FieldRef descriptor for the field "name" in this module, 
        /// if one exists
        /// </summary>
        /// <param name="name">field name</param>
        /// <returns>FieldRef descriptor for "name" or null</returns>
        public FieldRef GetField(string name)
        {
            return defaultClass.GetField(name);
        }

        /// <summary>
        /// Get all the fields of this module
        /// </summary>
        /// <returns>Array of FieldRefs for this module</returns>
        public FieldRef[] GetFields()
        {
            return defaultClass.GetFields();
        }

        internal void AddToMethodList(MethodRef meth)
        {
            defaultClass.AddToMethodList(meth);
        }

        internal void AddToFieldList(FieldRef fld)
        {
            defaultClass.AddToFieldList(fld);
        }

        internal MethodRef GetMethod(MethSig mSig)
        {
            return (MethodRef)defaultClass.GetMethod(mSig);
        }


    }
    /**************************************************************************/
    /// <summary>
    /// A reference to an external assembly (.assembly extern)
    /// </summary>
    public class AssemblyRef : ReferenceScope {
        private ushort major, minor, build, revision;
        uint flags, keyIx, hashIx, cultIx;
        bool hasVersion = false, isKeyToken = false;
        byte[] keyBytes, hashBytes;
        string culture;

        /*-------------------- Constructors ---------------------------------*/

        internal AssemblyRef(string name)
            : base(name)
        {
            tabIx = MDTable.AssemblyRef;
        }

        internal AssemblyRef(string name, ushort maj, ushort min, ushort bldNo, ushort rev,
          uint flags, byte[] kBytes, string cult, byte[] hBytes)
            : base(name)
        {
            tabIx = MDTable.AssemblyRef;
            major = maj;
            minor = min;
            build = bldNo;
            revision = rev;
            this.flags = flags;  // check
            keyBytes = kBytes;  // need to set is token or full key
            if(keyBytes != null)
                isKeyToken = keyBytes.Length <= 8;
            culture = cult;
            hashBytes = hBytes;
            tabIx = MDTable.AssemblyRef;
        }

        internal static AssemblyRef Read(PEReader buff)
        {
            ushort majVer = buff.ReadUInt16();
            ushort minVer = buff.ReadUInt16();
            ushort bldNo = buff.ReadUInt16();
            ushort revNo = buff.ReadUInt16();
            uint flags = buff.ReadUInt32();
            byte[] pKey =  buff.GetBlob();
            string name = buff.GetString();
            string cult = buff.GetString();
            byte[] hBytes = buff.GetBlob();
            AssemblyRef assemRef;
            if(name.ToLower() == "mscorlib") {
                assemRef = MSCorLib.mscorlib;
                assemRef.AddVersionInfo(majVer, minVer, bldNo, revNo);
                assemRef.AddHash(hBytes);
                if(pKey.Length > 8)
                    assemRef.AddKey(pKey);
                else
                    assemRef.AddKeyToken(pKey);
                assemRef.AddCulture(cult);
                assemRef.SetFlags(flags);
            } else {
                assemRef = new AssemblyRef(name, majVer, minVer, bldNo, revNo, flags, pKey, cult, hBytes);
            }
            return assemRef;
        }

        internal static void Read(PEReader buff, TableRow[] table)
        {
            for(int i=0; i < table.Length; i++)
                table[i] = Read(buff);
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Add version information about this external assembly
        /// </summary>
        /// <param name="majVer">Major Version</param>
        /// <param name="minVer">Minor Version</param>
        /// <param name="bldNo">Build Number</param>
        /// <param name="revNo">Revision Number</param>
        public void AddVersionInfo(int majVer, int minVer, int bldNo, int revNo)
        {
            major = (ushort)majVer;
            minor = (ushort)minVer;
            build = (ushort)bldNo;
            revision = (ushort)revNo;
            hasVersion = true;
        }

        /// <summary>
        /// Get the major version for this external assembly
        /// </summary>
        /// <returns>major version number</returns>
        public int MajorVersion()
        {
            return major;
        }
        /// <summary>
        /// Get the minor version for this external assembly
        /// </summary>
        /// <returns>minor version number</returns>
        public int MinorVersion()
        {
            return minor;
        }
        /// <summary>
        /// Get the build number for this external assembly
        /// </summary>
        /// <returns>build number</returns>
        public int BuildNumber()
        {
            return build;
        }
        /// <summary>
        /// Get the revision number for this external assembly
        /// </summary>
        /// <returns>revision number</returns>
        public int RevisionNumber()
        {
            return revision;
        }

        /// <summary>
        /// Check if this external assembly has any version information
        /// </summary>
        public bool HasVersionInfo()
        {
            return hasVersion;
        }

        /// <summary>
        /// Add the hash value for this external assembly
        /// </summary>
        /// <param name="hash">bytes of the hash value</param>
        public void AddHash(byte[] hash)
        {
            hashBytes = hash;
        }

        /// <summary>
        /// Get the hash value for this external assembly
        /// </summary>
        /// <returns></returns>
        public byte[] GetHash()
        {
            return hashBytes;
        }

        /// <summary>
        /// Set the culture for this external assembly
        /// </summary>
        /// <param name="cult">the culture string</param>
        public void AddCulture(string cult)
        {
            culture = cult;
        }

        public string GetCulture()
        {
            return culture;
        }

        /// <summary>
        /// Add the full public key for this external assembly
        /// </summary>
        /// <param name="key">bytes of the public key</param>
        public void AddKey(byte[] key)
        {
            flags |= 0x0001;   // full public key
            keyBytes = key;
        }

        /// <summary>
        /// Add the public key token (low 8 bytes of the public key)
        /// </summary>
        /// <param name="key">low 8 bytes of public key</param>
        public void AddKeyToken(byte[] key)
        {
            keyBytes = key;
            isKeyToken = true;
        }

        /// <summary>
        /// Get the public key token
        /// </summary>
        /// <returns>bytes of public key</returns>
        public byte[] GetKey()
        {
            return keyBytes;
        }

        /// <summary>
        /// Make an AssemblyRef for "name".  
        /// </summary>
        /// <param name="name">The name of the assembly</param>
        /// <returns>AssemblyRef for "name".</returns>
        public static AssemblyRef MakeAssemblyRef(string name)
        {
            AssemblyRef assemRef =  new AssemblyRef(name);
            return assemRef;
        }

        /*------------------------ internal functions ----------------------------*/

        internal void SetFlags(uint flags)
        {
            this.flags = flags;
        }

        internal string AssemblyString()
        {
            string result = name;
            if(hasVersion)
                result = result + ", Version=" + major + "." + minor + "." + 
          build + "." + revision;
            if(keyBytes != null) {
                string tokenStr = "=";
                if(isKeyToken)
                    tokenStr = "Token=";
                result = result + ", PublicKey" + tokenStr;
                for(int i=0; i < keyBytes.Length; i++) {
                    result = result + Hex.Byte(keyBytes[i]);
                }
            }
            if(culture != null)
                result = result + ", Culture=" + culture;
            return result;
        }

        internal static uint Size(MetaData md)
        {
            return 12 + 2 * md.StringsIndexSize() + 2 * md.BlobIndexSize();
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.AssemblyRef, this);
            keyIx = md.AddToBlobHeap(keyBytes);
            nameIx = md.AddToStringsHeap(name);
            cultIx = md.AddToStringsHeap(culture);
            hashIx = md.AddToBlobHeap(hashBytes);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(major);
            output.Write(minor);
            output.Write(build);
            output.Write(revision);
            output.Write(flags);
            output.BlobIndex(keyIx);
            output.StringsIndex(nameIx);
            output.StringsIndex(cultIx);
            output.BlobIndex(hashIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.ResolutionScope):
                return 2;
            case (CIx.HasCustomAttr):
                return 15;
            case (CIx.Implementation):
                return 1;
            }
            return 0;
        }

    }

    /**************************************************************************/
    /// <summary>
    /// The assembly for mscorlib.  
    /// </summary>
    public sealed class MSCorLib : AssemblyRef {
        internal static MSCorLib mscorlib = new MSCorLib();
        internal SystemClass ObjectClass;
        private ClassRef valueType;

        internal MSCorLib()
            : base("mscorlib")
        {
            classes.Add(new SystemClass(this, PrimitiveType.Void));
            classes.Add(new SystemClass(this, PrimitiveType.Boolean));
            classes.Add(new SystemClass(this, PrimitiveType.Char));
            classes.Add(new SystemClass(this, PrimitiveType.Int8));
            classes.Add(new SystemClass(this, PrimitiveType.UInt8));
            classes.Add(new SystemClass(this, PrimitiveType.Int16));
            classes.Add(new SystemClass(this, PrimitiveType.UInt16));
            classes.Add(new SystemClass(this, PrimitiveType.Int32));
            classes.Add(new SystemClass(this, PrimitiveType.UInt32));
            classes.Add(new SystemClass(this, PrimitiveType.Int64));
            classes.Add(new SystemClass(this, PrimitiveType.UInt64));
            classes.Add(new SystemClass(this, PrimitiveType.Float32));
            classes.Add(new SystemClass(this, PrimitiveType.Float64));
            classes.Add(new SystemClass(this, PrimitiveType.IntPtr));
            classes.Add(new SystemClass(this, PrimitiveType.UIntPtr));
            classes.Add(new SystemClass(this, PrimitiveType.String));
            classes.Add(new SystemClass(this, PrimitiveType.TypedRef));
            ObjectClass = new SystemClass(this, PrimitiveType.Object);
            classes.Add(ObjectClass);
            valueType = new ClassRef(this, "System", "ValueType");
            valueType.MakeValueClass();
            classes.Add(valueType);
        }

        internal ClassRef ValueType()
        {
            return valueType;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a module in an assembly
    /// </summary>
    public class ModuleRef : ReferenceScope {
        ArrayList exportedClasses = new ArrayList();
        internal ModuleFile modFile;
        internal Module defOf;
        internal bool ismscorlib = false;

        /*-------------------- Constructors ---------------------------------*/

        internal ModuleRef(string name, bool entryPoint, byte[] hashValue)
            : base(name)
        {
            modFile = new ModuleFile(name, hashValue, entryPoint);
            ismscorlib = name.ToLower() == "mscorlib.dll";
            tabIx = MDTable.ModuleRef;
        }

        internal ModuleRef(string name)
            : base(name)
        {
            ismscorlib = name.ToLower() == "mscorlib.dll";
            tabIx = MDTable.ModuleRef;
        }

        internal ModuleRef(ModuleFile file)
            : base(file.Name())
        {
            modFile = file;
            tabIx = MDTable.ModuleRef;
        }

        internal static void Read(PEReader buff, TableRow[] mods, bool resolve)
        {
            for(int i=0; i < mods.Length; i++) {
                string name = buff.GetString();
                ModuleRef mRef = new ModuleRef(name);
                if(resolve)
                    mRef.modFile = buff.GetFileDesc(name);
                mods[i] = mRef;
            }
        }

        internal sealed override void Resolve(PEReader buff)
        {
            modFile = buff.GetFileDesc(name);
            if(modFile != null)
                modFile.fileModule = this;
        }

        /*------------------------- public set and get methods --------------------------*/


        /// <summary>
        /// Add a class which is declared public in this external module of
        /// THIS assembly.  This class will be exported from this assembly.
        /// The ilasm syntax for this is .extern class
        /// </summary>
        /// <param name="attrSet">attributes of the class to be exported</param>
        /// <param name="nsName">name space name</param>
        /// <param name="name">external class name</param>
        /// <param name="declFile">the file where the class is declared</param>
        /// <param name="isValueClass">is this class a value type?</param>
        /// <returns>a descriptor for this external class</returns>
        public ClassRef AddExternClass(TypeAttr attrSet, string nsName,
          string name, bool isValueClass, PEFile pefile)
        {
            ClassRef cRef = new ClassRef(this, nsName, name);
            if(isValueClass)
                cRef.MakeValueClass();
            ExternClass eClass = new ExternClass(attrSet, nsName, name, modFile);
            exportedClasses.Add(eClass);
            cRef.SetExternClass(eClass);
            classes.Add(cRef);
            return cRef;
        }

        public static ModuleRef MakeModuleRef(string name, bool entryPoint, byte[] hashValue)
        {
            ModuleRef mRef = new ModuleRef(name, entryPoint, hashValue);
            return mRef;
        }

        public void SetEntryPoint()
        {
            modFile.SetEntryPoint();
        }

        public void SetHash(byte[] hashVal)
        {
            modFile.SetHash(hashVal);
        }

        /*------------------------- internal functions --------------------------*/

        /*    internal void AddMember(Member memb) {
              if (memb is Method) {
                Method existing = GetMethod(memb.Name(),((Method)memb).GetParTypes());
                if (existing == null) 
                  methods.Add(memb);
              } else {
                Field existing = GetField(memb.Name());
                if (existing == null)
                  fields.Add(memb);
              }
            }
            */

        internal void AddToExportedClassList(ClassRef exClass)
        {
            if(exportedClasses.Contains(exClass))
                return;
            exportedClasses.Add(exClass);
        }

        internal void AddExternClass(ExternClass eClass)
        {
            exportedClasses.Add(eClass);
        }

        internal static uint Size(MetaData md)
        {
            return md.StringsIndexSize();
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.ModuleRef, this);
            nameIx = md.AddToStringsHeap(name);
            if(modFile != null)
                modFile.BuildMDTables(md);
            for(int i=0; i < exportedClasses.Count; i++)
                ((ExternClass)exportedClasses[i]).BuildMDTables(md);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.StringsIndex(nameIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.HasCustomAttr):
                return 12;
            case (CIx.MemberRefParent):
                return 2;
            case (CIx.ResolutionScope):
                return 1;
            }
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Base descriptor for signature blobs
    /// </summary>
    public class Signature : MetaDataElement {
        protected uint sigIx;
        protected byte[] sigBytes;

        /*-------------------- Constructors ---------------------------------*/

        internal Signature()
        {
            tabIx = MDTable.StandAloneSig;
        }

        private Signature(uint sIx)
        {
            sigIx = sIx;
        }

        internal static void Read(PEReader buff, TableRow[] sigs)
        {
            for(int i=0; i < sigs.Length; i++) {
                uint sigIx = buff.GetBlobIx();
                uint tag = buff.FirstBlobByte(sigIx);
                if(tag == LocalSig.LocalSigByte)
                    sigs[i] = new LocalSig(sigIx);
                else if(tag == Field.FieldTag)
                    sigs[i] = new Signature(sigIx);
                else
                    sigs[i] = new CalliSig(sigIx);
                sigs[i].Row = (uint)i+1;
            }
        }

        internal override void Resolve(PEReader buff)
        {
            Type sigType = buff.GetFieldType(sigIx);
            buff.ReplaceSig(this, sigType);
        }

        internal static uint Size(MetaData md)
        {
            return md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            output.BlobIndex(sigIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            return (uint)tabIx;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Signature for calli instruction
    /// </summary>
    public class CalliSig : Signature {
        CallConv callConv;
        Type retType;
        Type[] parTypes, optParTypes;
        uint numPars = 0, numOptPars = 0;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a signature for a calli instruction
        /// </summary>
        /// <param name="cconv">calling conventions</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameter types</param>
        public CalliSig(CallConv cconv, Type retType, Type[] pars)
        {
            callConv = cconv;
            this.retType = retType;
            parTypes = pars;
            if(pars != null)
                numPars = (uint)pars.Length;
        }

        internal CalliSig(uint sigIx)
        {
            this.sigIx = sigIx;
        }

        internal sealed override void Resolve(PEReader buff)
        {
            MethSig mSig = buff.ReadMethSig(null, sigIx);
            callConv = mSig.callConv;
            retType = mSig.retType;
            parTypes = mSig.parTypes;
            if(parTypes != null)
                numPars = (uint)parTypes.Length;
            optParTypes = mSig.optParTypes;
            if(optParTypes != null)
                numOptPars = (uint)optParTypes.Length;
        }

        /// <summary>
        /// Add the optional parameters to a vararg method
        /// This method sets the vararg calling convention
        /// </summary>
        /// <param name="optPars">the optional pars for the vararg call</param>
        public void AddVarArgs(Type[] optPars)
        {
            optParTypes = optPars;
            if(optPars != null)
                numOptPars = (uint)optPars.Length;
            callConv |= CallConv.Vararg;
        }

        /// <summary>
        /// Add extra calling conventions to this callsite signature
        /// </summary>
        /// <param name="cconv"></param>
        public void AddCallingConv(CallConv cconv)
        {
            callConv |= cconv;
        }

        internal void ChangeRefsToDefs(ClassDef newType, ClassDef[] oldTypes)
        {
            for(int i=0; i < oldTypes.Length; i++) {
                if(retType == oldTypes[i])
                    retType = newType;
                for(int j=0; j < numPars; j++) {
                    if(parTypes[j] == oldTypes[i])
                        parTypes[j] = newType;
                }
                for(int j=0; j < numOptPars; j++) {
                    if(optParTypes[j] == oldTypes[i])
                        optParTypes[j] = newType;
                }
            }
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.StandAloneSig, this);
            for(int i=0; i < numPars; i++) {
                parTypes[i].BuildMDTables(md);
            }
            if(numOptPars > 0) {
                for(int i=0; i < numOptPars; i++) {
                    optParTypes[i].BuildMDTables(md);
                }
            }
        }

        internal sealed override void BuildSignatures(MetaDataOut md)
        {
            MemoryStream sig = new MemoryStream();
            sig.WriteByte((byte)callConv);
            MetaDataOut.CompressNum(numPars+numOptPars, sig);
            retType.TypeSig(sig);
            for(int i=0; i < numPars; i++) {
                parTypes[i].TypeSig(sig);
            }
            if(numOptPars > 0) {
                sig.WriteByte((byte)ElementType.Sentinel);
                for(int i=0; i < numOptPars; i++) {
                    optParTypes[i].TypeSig(sig);
                }
            }
            sigIx = md.AddToBlobHeap(sig.ToArray());
            done = false;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for the locals for a method
    /// </summary>
    public class LocalSig : Signature {
        internal static readonly byte LocalSigByte = 0x7;
        Local[] locals;
        bool resolved = true;

        /*-------------------- Constructors ---------------------------------*/

        public LocalSig(Local[] locals)
        {
            this.locals = locals;
        }

        internal LocalSig(uint sigIx)
        {
            resolved = false;
            this.sigIx = sigIx;
        }

        internal override void Resolve(PEReader buff)
        {
        }

        internal void Resolve(PEReader buff, MethodDef meth)
        {
            if(resolved)
                return;
            buff.currentMethodScope = meth;
            buff.currentClassScope = (Class)meth.GetParent();
            locals = buff.ReadLocalSig(sigIx);
            buff.currentMethodScope = null;
            buff.currentClassScope = null;
        }

        internal Local[] GetLocals()
        {
            return locals;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(tabIx, this);
            for(int i=0; i < locals.Length; i++) {
                locals[i].BuildTables(md);
            }
        }

        internal byte[] SigBytes()
        {
            MemoryStream sig = new MemoryStream();
            sig.WriteByte(LocalSigByte);
            MetaDataOut.CompressNum((uint)locals.Length, sig);
            for(int i=0; i < locals.Length; i++) {
                ((Local)locals[i]).TypeSig(sig);
            }
            return sig.ToArray();
        }

        internal sealed override void BuildSignatures(MetaDataOut md)
        {
            sigIx = md.AddToBlobHeap(SigBytes());
            done = false;
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Base class for all IL types
    /// </summary>
    public abstract class Type : MetaDataElement {
        protected byte typeIndex;

        /*-------------------- Constructors ---------------------------------*/

        internal Type(byte tyIx)
        {
            typeIndex = tyIx;
        }

        internal byte GetTypeIndex()
        {
            return typeIndex;
        }

        internal virtual bool SameType(Type tstType)
        {
            return this == tstType;
        }

        internal virtual void TypeSig(MemoryStream str)
        {
            throw new TypeSignatureException(this.GetType().AssemblyQualifiedName +
        " doesn't have a type signature!!");
        }

        public virtual string TypeName()
        {
            return "NoTypeName";
        }

        internal virtual Type AddTypeSpec(MetaDataOut md)
        {
            if(!isDef())
                BuildMDTables(md);
            return this;
        }

    }

    /**************************************************************************/
    /*
      internal class IndexedType : Type {
        uint ix;
        CIx codedIx;
        bool isCoded = false;

        internal IndexedType(PEReader buff, MDTable tIx, uint elIx) {
          buffer = buff;
          tabIx = tIx;
          elIx = ix;
        }

        internal IndexedType(PEReader buff, CIx cIx, uint elIx) {
          buffer = buff;
          codedIx = cIx;
          ix = elIx;
          isCoded = true;
        }

        internal Type Resolve() {
          if (isCoded)
            return buffer.ReadCodedElement(cIx,ix);
          return buffer.ReadElement(tabIx,ix);
        }

      }
      */

    /**************************************************************************/
    /// <summary>
    /// The base descriptor for a class 
    /// </summary>
    public abstract class Class : Type {
        //protected int row = 0;
        protected string name, nameSpace;
        protected uint nameIx, nameSpaceIx;
        protected ArrayList nestedClasses = new ArrayList();
        protected bool special = false;
        protected ArrayList fields = new ArrayList();
        protected ArrayList methods = new ArrayList();
        internal uint fieldIx = 0, methodIx = 0, fieldEndIx = 0, methodEndIx = 0;
        protected string[] fieldNames, methodNames;
        protected ArrayList genericParams = new ArrayList();

        /*-------------------- Constructors ---------------------------------*/

        internal Class()
            : base((byte)ElementType.Class)
        {
        }

        /*------------------------- public set and get methods --------------------------*/

        public virtual void MakeValueClass()
        {
            typeIndex = (byte)ElementType.ValueType;
        }

        public string Name()
        {
            return name;
        }

        public string NameSpace()
        {
            return nameSpace;
        }

        public override string TypeName()
        {
            if((nameSpace == null) || (nameSpace == ""))
                return name;
            return nameSpace + "." + name;
        }

        /// <summary>
        /// Get the descriptor for the method "name" of this class
        /// </summary>
        /// <param name="name">The name of the method to be retrieved</param>
        /// <returns>The method descriptor for "name"</returns>
        public Method GetMethodDesc(string name)
        {
            for(int i=0; i < methods.Count; i++) {
                if(((Method)methods[i]).HasName(name))
                    return (Method)methods[i];
            }
            return null;
        }

        /// <summary>
        /// Get the descriptor for the method called "name" with the signature "parTypes"
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="parTypes">The signature of the method</param>
        /// <returns>The method descriptor for name(parTypes)</returns>
        public Method GetMethodDesc(string name, Type[] parTypes)
        {
            for(int i=0; i < methods.Count; i++) {
                if(((Method)methods[i]).HasNameAndSig(name, parTypes))
                    return (Method)methods[i];
            }
            return null;
        }

        /// <summary>
        /// Get the vararg method "name(parTypes,optTypes)" for this class
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="parTypes">Method parameter types</param>
        /// <param name="optTypes">Optional parameter types</param>
        /// <returns>Descriptor for "name(parTypes,optTypes)"</returns>
        public Method GetMethodDesc(string name, Type[] parTypes, Type[] optParTypes)
        {
            for(int i=0; i < methods.Count; i++) {
                if(((Method)methods[i]).HasNameAndSig(name, parTypes, optParTypes))
                    return (Method)methods[i];
            }
            return null;
        }

        /// <summary>
        /// Get all the methods of this class called "name"
        /// </summary>
        /// <param name="name">The method name</param>
        /// <returns>List of methods called "name"</returns>
        public Method[] GetMethodDescs(string name)
        {
            ArrayList meths = GetMeths(name);
            return (Method[])meths.ToArray(typeof(Method));
        }

        /// <summary>
        /// Get all the methods for this class
        /// </summary>
        /// <returns>List of methods for this class</returns>
        public Method[] GetMethodDescs()
        {
            return (Method[])methods.ToArray(typeof(Method));
        }

        public void RemoveMethod(string name)
        {
            Method meth = GetMethodDesc(name);
            if(meth != null)
                methods.Remove(meth);
        }

        public void RemoveMethod(string name, Type[] parTypes)
        {
            Method meth = GetMethodDesc(name, parTypes);
            if(meth != null)
                methods.Remove(meth);
        }

        public void RemoveMethod(string name, Type[] parTypes, Type[] optTypes)
        {
            Method meth = GetMethodDesc(name, parTypes, optTypes);
            if(meth != null)
                methods.Remove(meth);
        }

        public void RemoveMethod(Method meth)
        {
            methods.Remove(meth);
        }

        public void RemoveMethod(int ix)
        {
            methods.RemoveAt(ix);
        }

        /// <summary>
        /// Get the descriptor for the field "name" for this class
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Descriptor for field "name"</returns>
        public Field GetFieldDesc(string name)
        {
            return FindField(name);
        }

        /// <summary>
        /// Get all the fields for this class
        /// </summary>
        /// <returns>List of fields for this class</returns>
        public Field[] GetFieldDescs()
        {
            return (Field[])fields.ToArray(typeof(Field));
        }

        public void RemoveField(string name)
        {
            Field f = FindField(name);
            if(f != null)
                fields.Remove(f);
        }

        public virtual ClassSpec Instantiate(Type[] genTypes)
        {
            return new ClassSpec(this, genTypes);
        }

        public virtual void MakeSpecial()
        {
            special = true;
        }

        public abstract MetaDataElement GetParent();

        public Class[] GetNestedClasses()
        {
            return (Class[])nestedClasses.ToArray(typeof(Class));
        }

        public int GetNestedClassCount()
        {
            return nestedClasses.Count;
        }

        /*------------------------- internal functions --------------------------*/

        internal virtual Type GetGenPar(uint ix)
        {
            return null;
        }

        protected ArrayList GetMeths(string name)
        {
            ArrayList meths = new ArrayList();
            for(int i=0; i < methods.Count; i++) {
                if(((Method)methods[i]).HasName(name))
                    meths.Add(methods[i]);
            }
            return meths;
        }

        internal ArrayList GetFieldList()
        {
            return fields;
        }

        internal ArrayList GetMethodList()
        {
            return methods;
        }

        internal bool isValueType()
        {
            return typeIndex == (byte)ElementType.ValueType;
        }

        internal bool isSpecial()
        {
            return special;
        }

        internal void AddToFieldList(Field f)
        {
            f.SetParent(this);
            fields.Add(f);
        }

        internal void AddToList(ArrayList list, MDTable tabIx)
        {
            switch(tabIx) {
            case (MDTable.Field):
                fields.AddRange(list);
                break;
            case (MDTable.Method):
                methods.AddRange(list);
                break;
            case (MDTable.TypeDef):
                nestedClasses.AddRange(list);
                break;
            default:
                throw new Exception("Unknown list type");
            }
        }

        internal void AddToMethodList(Method m)
        {
            m.SetParent(this);
            methods.Add(m);
        }

        internal void AddToClassList(Class nClass)
        {
            nestedClasses.Add(nClass);
        }

        internal Class GetNested(string name)
        {
            for(int i=0; i < nestedClasses.Count; i++) {
                if(((Class)nestedClasses[i]).Name() == name)
                    return (Class)nestedClasses[i];
            }
            return null;
        }

        internal Method GetMethod(MethSig mSig)
        {
            return GetMethodDesc(mSig.name, mSig.parTypes, mSig.optParTypes);
        }

        protected Field FindField(string name)
        {
            for(int i=0; i < fields.Count; i++) {
                if(((Field)fields[i]).Name() == name)
                    return (Field)fields[i];
            }
            return null;
        }

        internal void SetBuffer(PEReader buff)
        {
            buffer = buff;
        }

        internal override void TypeSig(MemoryStream sig)
        {
            sig.WriteByte(typeIndex);
            MetaDataOut.CompressNum(TypeDefOrRefToken(), sig);
        }

        internal abstract string ClassName();

        internal virtual uint TypeDefOrRefToken()
        {
            return 0;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// 
    /// </summary> 
    public class ClassSpec : Class {
        Class genClass;
        uint sigIx;
        private static byte GENERICINST = 0x15;

        /*-------------------- Constructors ---------------------------------*/

        internal ClassSpec(Class clType, Type[] gPars)
        {
            this.typeIndex = GENERICINST;
            genClass = clType;
            genericParams = new ArrayList(gPars);
            tabIx = MDTable.TypeSpec;
            typeIndex = GENERICINST;
            ArrayList classMethods = clType.GetMethodList();
            ArrayList classFields = clType.GetFieldList();
            for(int i=0; i < classMethods.Count; i++) {
                MethSig mSig = ((Method)classMethods[i]).GetSig(); //.InstantiateGenTypes(this,gPars);
                if(mSig != null) {
                    MethodRef newMeth = new MethodRef(mSig);
                    newMeth.SetParent(this);
                    newMeth.GenericParams = ((Method)classMethods[i]).GenericParams;
                    methods.Add(newMeth);
                }
            }
            for(int i=0; i < classFields.Count; i++) {
                Type fType = ((Field)classFields[i]).GetFieldType();
                //if ((fType is GenericParam) && (((GenericParam)fType).GetParent() == genClass)) {
                //  fType = gPars[((GenericParam)fType).Index];
                //}
                fields.Add(new FieldRef(this, ((Field)classFields[i]).Name(), fType));
            }
        }

        /*------------------------- public set and get methods --------------------------*/

        public override MetaDataElement GetParent()
        {
            return null;
        }

        internal override string ClassName()
        {
            // need to return something here??
            return null;
        }

        public Type GetGenericParamType(int ix)
        {
            if(ix >= genericParams.Count)
                return null;
            return (Type)genericParams[ix];
        }

        public Type[] GetGenericParamTypes()
        {
            return (Type[])genericParams.ToArray(typeof(Type));
        }

        public Class GetGenericClass()
        {
            return genClass;
        }

        public int GetGenericParCount()
        {
            return genericParams.Count;
        }

        /*----------------------------- internal functions ------------------------------*/

        internal void AddMethod(Method meth)
        {
            methods.Add(meth);
            meth.SetParent(this);
        }

        internal override sealed uint TypeDefOrRefToken()
        {
            uint cIx = Row;
            cIx = (cIx << 2) | 0x2;
            return cIx;
        }

        internal override Type GetGenPar(uint ix)
        {
            if(genClass == null)
                return new GenericParam(null, this, (int)ix);
            return genClass.GetGenPar(ix);
            //if (ix >= genericParams.Count) return null;
            //return (Type)genericParams[(int)ix];
        }

        internal override sealed Type AddTypeSpec(MetaDataOut md)
        {
            md.AddToTable(MDTable.TypeSpec, this);
            BuildMDTables(md);
            return this;
        }

        internal override void BuildTables(MetaDataOut md)
        {
            //md.AddToTable(MDTable.TypeSpec,this);
            if(!genClass.isDef())
                genClass.BuildMDTables(md);
            for(int i=0; i < genericParams.Count; i++) {
                if(!((Type)genericParams[i]).isDef() && 
           (!(genericParams[i] is GenericParam)))
                    ((Type)genericParams[i]).BuildMDTables(md);
            }
        }

        internal override void BuildSignatures(MetaDataOut md)
        {
            MemoryStream outSig = new MemoryStream();
            TypeSig(outSig);
            sigIx = md.AddToBlobHeap(outSig.ToArray());
        }

        internal sealed override void TypeSig(MemoryStream sig)
        {
            sig.WriteByte(typeIndex);
            genClass.TypeSig(sig);
            MetaDataOut.CompressNum((uint)genericParams.Count, sig);
            for(int i=0; i < genericParams.Count; i++) {
                ((Type)genericParams[i]).TypeSig(sig);
            }
        }

        internal sealed override void Write(PEWriter output)
        {
            //Console.WriteLine("Writing the blob index for a TypeSpec");
            output.BlobIndex(sigIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.TypeDefOrRef):
                return 2;
            case (CIx.HasCustomAttr):
                return 13;
            case (CIx.MemberRefParent):
                return 4;
            }
            return 0;
        }
    }

    /**************************************************************************/
    /// <summary>
    /// wrapper for TypeSpec parent of MethodRef or FieldRef
    /// </summary>
    public class ConstructedTypeSpec : Class {
        TypeSpec constrType;

        public ConstructedTypeSpec(TypeSpec tySpec)
            : base()
        {
            constrType = tySpec;
            this.typeIndex = constrType.GetTypeIndex();
        }

        public override MetaDataElement GetParent()
        {
            return null;
        }

        internal override string ClassName()
        {
            return constrType.NameString();
        }



    }

    /**************************************************************************/
    public abstract class ClassDesc : Class {

        /*-------------------- Constructors ---------------------------------*/

        internal ClassDesc(string nameSpaceName, string className)
        {
            nameSpace = nameSpaceName;
            name = className;
        }

        internal ClassDesc()
        {
        }

        /*------------------------- public set and get methods --------------------------*/

        public GenericParam GetGenericParam(int ix)
        {
            if(ix >= genericParams.Count)
                return null;
            return (GenericParam)genericParams[ix];
        }

        public GenericParam[] GetGenericParams()
        {
            return (GenericParam[])genericParams.ToArray(typeof(GenericParam));
        }

        public virtual void SetGenericParams(GenericParam[] genPars)
        {
            for(int i=0; i < genPars.Length; i++) {
                genPars[i].SetClassParam(this, i);
            }
            genericParams = new ArrayList(genPars);
        }

        /*----------------------------- internal functions ------------------------------*/

        protected void DeleteGenericParam(int pos)
        {
            genericParams.RemoveAt(pos);
            for(int i=pos; i < genericParams.Count; i++) {
                GenericParam gp = (GenericParam)genericParams[i];
                gp.Index = (uint)i;
            }
        }

        internal void AddGenericParam(GenericParam par)
        {
            genericParams.Add(par);
            //par.SetClassParam(this,genericParams.Count-1);
        }

        internal override Type GetGenPar(uint ix)
        {
            // create generic param descriptor if one does not exist
            // - used when reading exported interface
            return (GenericParam)genericParams[(int)ix];
        }

    }

    /**************************************************************************/
    // This Class produces entries in the TypeDef table of the MetaData 
    // in the PE meta data.

    // NOTE:  Entry 0 in TypeDef table is always the pseudo class <module> 
    // which is the parent for functions and variables declared a module level

    /// <summary>
    /// The descriptor for a class defined in the IL (.class) in the current assembly/module
    /// </summary>
    /// 
    public class ClassDef : ClassDesc {
        private static readonly uint HasSecurity = 0x00040000;
        private static readonly uint NoSecurity = 0xFFFBFFFF;
        private static readonly uint VisibilityMask = 0x07;
        //private static readonly uint LayoutMask = 0x18;
        //private static readonly uint fieldListIx = 0, methListIx = 1, eventListIx = 2, propListIx = 3; 
        //private static readonly uint	numListIx = 4;

        protected PEFile scope;
        uint flags;
        Class superType;
        ArrayList security;
        ClassLayout layout;
        uint extendsIx;
        internal ClassRef refOf;
        internal uint eventIx = 0, propIx = 0;
        ArrayList events = new ArrayList();
        ArrayList properties = new ArrayList();
        ArrayList interfaces = new ArrayList();
        ArrayList methodImpls = new ArrayList();
        //uint[] interfaceIndexes;
        //private string[] eventNames, propertyNames, nestedNames;
        //internal string[][] names = new string[numListIx][];

        /*-------------------- Constructors ---------------------------------*/

        internal ClassDef(PEFile scope, TypeAttr attrSet, string nsName, string name)
            : base(nsName, name)
        {
            this.scope = scope;
            superType = MSCorLib.mscorlib.ObjectClass;
            flags = (uint)attrSet;
            tabIx = MDTable.TypeDef;
        }

        internal ClassDef(PEReader buff, uint row, bool isMSCorLib)
        {
            flags = buff.ReadUInt32();
            name = buff.GetString();
            nameSpace = buff.GetString();
            extendsIx = buff.GetCodedIndex(CIx.TypeDefOrRef);
            fieldIx = buff.GetIndex(MDTable.Field);
            methodIx = buff.GetIndex(MDTable.Method);
            this.Row = row;
            tabIx = MDTable.TypeDef;
            if(isMSCorLib && (name == "ValueType"))
                typeIndex = (byte)ElementType.ValueType;
        }

        internal static void Read(PEReader buff, TableRow[] typeDefs, bool isMSCorLib)
        {
            ClassDef prevDef = null;
            prevDef = new ClassDef(buff, 1, isMSCorLib);
            typeDefs[0] = prevDef;
            for(int i=1; i < typeDefs.Length; i++) {
                ClassDef typeDef = new ClassDef(buff, (uint)i+1, isMSCorLib);
                prevDef.fieldEndIx = typeDef.fieldIx;
                prevDef.methodEndIx = typeDef.methodIx;
                prevDef = typeDef;
                typeDefs[i] = typeDef;
            }
            prevDef.fieldEndIx = buff.GetTableSize(MDTable.Field)+1;
            prevDef.methodEndIx = buff.GetTableSize(MDTable.Method)+1;
        }

        private static uint GetParentClassIx(uint[] enclClasses, uint[] nestClasses, uint classIx)
        {
            if(enclClasses == null)
                return 0;
            for(uint i=0; i < enclClasses.Length; i++) {
                if(nestClasses[i] == classIx)
                    return enclClasses[i];
            }
            return 0;
        }

        internal static void GetClassRefs(PEReader buff, TableRow[] typeRefs, ReferenceScope paren, uint[] parIxs)
        {
            int num = typeRefs.Length;
            uint[] fieldStart = new uint[num+1], methStart = new uint[num+1], extends = new uint[num+1];
            for(int i=0; i < num; i++) {
                uint flags = buff.ReadUInt32();
                string name = buff.GetString();
                string nameSpace = buff.GetString();
                extends[i] = buff.GetCodedIndex(CIx.TypeDefOrRef);
                fieldStart[i] = buff.GetIndex(MDTable.Field);
                methStart[i] = buff.GetIndex(MDTable.Method);
                //Console.WriteLine("flags = " + Hex.Int(flags));
                if(i == 0) // ASSERT first entry is always <Module>
                    typeRefs[i] = paren.GetDefaultClass();
                else if(isPublic(flags)) {
                    if(parIxs[i] != 0) {
                        typeRefs[i] = new NestedClassRef(paren, nameSpace, name);
                    } else {
                        typeRefs[i] = paren.GetExistingClass(nameSpace, name);
                        if(typeRefs[i] == null) {
                            typeRefs[i] = new ClassRef(paren, nameSpace, name);
                            paren.AddToClassList((ClassRef)typeRefs[i]);
                        }
                    }
                }
            }
            fieldStart[num] = buff.GetTableSize(MDTable.Field)+1;
            methStart[num] = buff.GetTableSize(MDTable.Method)+1;
            // Find Nested Classes
            for(int i=0; i < typeRefs.Length; i++) {
                if((typeRefs[i] != null) && (typeRefs[i] is NestedClassRef)) {
                    NestedClassRef nRef = (NestedClassRef)typeRefs[i];
                    ClassRef nPar = (ClassRef)typeRefs[parIxs[i]-1];
                    if(nPar == null) {  // parent is private, so ignore
                        typeRefs[i] = null;
                    } else {
                        nRef.SetParent(nPar);
                        nPar.AddToClassList(nRef);
                    }
                }
                if(typeRefs[i] != null) {
                    if(buff.GetCodedElement(CIx.TypeDefOrRef, extends[i]) == MSCorLib.mscorlib.ValueType())
                        ((ClassRef)typeRefs[i]).MakeValueClass();
                    buff.SetElementPosition(MDTable.Field, fieldStart[i]);
                    FieldDef.GetFieldRefs(buff, fieldStart[i+1]-fieldStart[i], (ClassRef)typeRefs[i]);
                    buff.SetElementPosition(MDTable.Method, methStart[i]);
                    MethodDef.GetMethodRefs(buff, methStart[i+1]-methStart[i], (ClassRef)typeRefs[i]);
                }
            }
        }

        internal override void Resolve(PEReader buff)
        {
            buff.currentClassScope = this;
            superType = (Class)buff.GetCodedElement(CIx.TypeDefOrRef, extendsIx);
            if((superType != null) && superType.isValueType())
                typeIndex = (byte)ElementType.ValueType;
            for(int i=0; fieldIx < fieldEndIx; i++, fieldIx++) {
                FieldDef field = (FieldDef)buff.GetElement(MDTable.Field, fieldIx);
                field.SetParent(this);
                fields.Add(field);
            }
            for(int i=0; methodIx < methodEndIx; i++, methodIx++) {
                MethodDef meth = (MethodDef)buff.GetElement(MDTable.Method, methodIx);
                if(Diag.DiagOn)
                    Console.WriteLine("Adding method " + meth.Name() + " to class " + name);
                meth.SetParent(this);
                methods.Add(meth);
            }
            buff.currentClassScope = null;
        }

        internal void ChangeRefsToDefs(ClassDef newType, ClassDef[] oldTypes)
        {
            for(int i=0; i < oldTypes.Length; i++) {
                for(int j=0; j < oldTypes[i].fields.Count; j++)
                    ((FieldDef)oldTypes[i].fields[j]).ChangeRefsToDefs(this, oldTypes);
                for(int j=0; j < oldTypes[i].methods.Count; j++)
                    ((MethodDef)oldTypes[i].methods[j]).ChangeRefsToDefs(this, oldTypes);
                for(int j=0; j < oldTypes[i].events.Count; j++)
                    ((Event)oldTypes[i].events[j]).ChangeRefsToDefs(this, oldTypes);
                for(int j=0; j < oldTypes[i].properties.Count; j++)
                    ((Property)oldTypes[i].properties[j]).ChangeRefsToDefs(this, oldTypes);
                for(int j=0; j < oldTypes[i].interfaces.Count; j++)
                    ((ClassDef)oldTypes[i].interfaces[j]).ChangeRefsToDefs(this, oldTypes);
                for(int j=0; j < oldTypes[i].methodImpls.Count; j++)
                    ((MethodImpl)oldTypes[i].methodImpls[j]).ChangeRefsToDefs(this, oldTypes);
                for(int j=0; j < oldTypes[i].nestedClasses.Count; j++)
                    ((ClassDef)oldTypes[i].nestedClasses[j]).ChangeRefsToDefs(this, oldTypes);
            }
        }

        public void MergeClasses(ClassDef[] classes)
        {
            ChangeRefsToDefs(this, classes);
            for(int i=0; i < classes.Length; i++) {
                fields.AddRange(classes[i].fields);
                methods.AddRange(classes[i].methods);
                events.AddRange(classes[i].events);
                properties.AddRange(classes[i].properties);
                interfaces.AddRange(classes[i].interfaces);
                methodImpls.AddRange(classes[i].methodImpls);
                nestedClasses.AddRange(classes[i].nestedClasses);
            }
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Fetch the PEFile which contains this class
        /// </summary>
        /// <returns>PEFile containing this class</returns>
        public virtual PEFile GetScope()
        {
            return scope;
        }

        public override MetaDataElement GetParent()
        {
            return scope;
        }

        /// <summary>
        /// Fetch or Get the superType for this class
        /// </summary>
        public Class SuperType
        {
            get
            {
                return superType;
            }
            set
            {
                superType = value;
                if(value == MSCorLib.mscorlib.ValueType())
                    typeIndex = (byte)ElementType.ValueType;
                else
                    typeIndex = (byte)ElementType.Class;
            }
        }

        /*    /// <summary>
            /// Make this class inherit from ValueType
            /// </summary>
            public override void MakeValueClass() {
              superType = MSCorLib.mscorlib.ValueType();
              typeIndex = (byte)ElementType.ValueType;
            }
            */

        /// <summary>
        /// Add an attribute to the attributes of this class
        /// </summary>
        /// <param name="ta">the attribute to be added</param>
        public void AddAttribute(TypeAttr ta)
        {
            flags |= (uint)ta;
        }

        /// <summary>
        /// Set the attributes of this class
        /// </summary>
        /// <param name="ta">class attributes</param>
        public void SetAttribute(TypeAttr ta)
        {
            flags = (uint)ta;
        }

        /// <summary>
        /// Get the attributes for this class
        /// </summary>
        /// <returns></returns>
        public TypeAttr GetAttributes()
        {
            return (TypeAttr)flags;
        }

        public GenericParam AddGenericParam(string name)
        {
            GenericParam gp = new GenericParam(name, this, genericParams.Count);
            genericParams.Add(gp);
            return gp;
        }

        public int GetGenericParamCount()
        {
            return genericParams.Count;
        }

        public GenericParam GetGenericParam(string name)
        {
            int pos = FindGenericParam(name);
            if(pos == -1)
                return null;
            return (GenericParam)genericParams[pos];
        }

        public void RemoveGenericParam(string name)
        {
            int pos = FindGenericParam(name);
            if(pos == -1)
                return;
            DeleteGenericParam(pos);
        }

        public void RemoveGenericParam(int ix)
        {
            if(ix >= genericParams.Count)
                return;
            DeleteGenericParam(ix);
        }

        public override ClassSpec Instantiate(Type[] genTypes)
        {
            if(genTypes == null)
                return null;
            if(genericParams.Count == 0)
                throw new Exception("Cannot instantiate non-generic class");
            if(genTypes.Length != genericParams.Count)
                throw new Exception("Wrong number of type parameters for instantiation\nNeeded " 
          + genericParams.Count + " but got " + genTypes.Length);
            return new ClassSpec(this, genTypes);
        }

        /// <summary>
        /// Add an interface that is implemented by this class
        /// </summary>
        /// <param name="iFace">the interface that is implemented</param>
        public void AddImplementedInterface(Class iFace)
        {
            interfaces.Add(new InterfaceImpl(this, iFace));
            //metaData.AddToTable(MDTable.InterfaceImpl,new InterfaceImpl(this,iFace));
        }

        /// <summary>
        /// Get the interfaces implemented by this class
        /// </summary>
        /// <returns>List of implemented interfaces</returns>
        public Class[] GetInterfaces()
        {
            Class[] iFaces = new Class[interfaces.Count];
            for(int i=0; i < iFaces.Length; i++) {
                iFaces[i] = ((InterfaceImpl)interfaces[i]).TheInterface();
            }
            return iFaces;
        }

        /// <summary>
        /// Add a field to this class
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="fType">field type</param>
        /// <returns>a descriptor for this new field</returns>
        public FieldDef AddField(string name, Type fType)
        {
            FieldDef field = (FieldDef)FindField(name);
            if(field != null)
                throw new DescriptorException("Field " + field.NameString());
            field = new FieldDef(name, fType, this);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Add a field to this class
        /// </summary>
        /// <param name="fAtts">attributes for this field</param>
        /// <param name="name">field name</param>
        /// <param name="fType">field type</param>
        /// <returns>a descriptor for this new field</returns>
        public FieldDef AddField(FieldAttr fAtts, string name, Type fType)
        {
            FieldDef field = AddField(name, fType);
            field.SetFieldAttr(fAtts);
            return field;
        }

        /// <summary>
        /// Add a field to this class
        /// </summary>
        /// <param name="f">Descriptor for the field to be added</param>
        public void AddField(FieldDef f)
        {
            FieldDef field = (FieldDef)FindField(f.Name());
            if(field != null)
                throw new DescriptorException("Field " + field.NameString());
            f.SetParent(this);
            fields.Add(f);
        }

        /// <summary>
        /// Get the descriptor for the field of this class named "name"
        /// </summary>
        /// <param name="name">The field name</param>
        /// <returns>The descriptor for field "name"</returns>
        public FieldDef GetField(string name)
        {
            return (FieldDef)FindField(name);
        }

        /// <summary>
        /// Get the fields for this class
        /// </summary>
        /// <returns>List of fields of this class</returns>
        public FieldDef[] GetFields()
        {
            return (FieldDef[])fields.ToArray(typeof(FieldDef));
        }

        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameters</param>
        /// <returns>a descriptor for this new method</returns>
        public MethodDef AddMethod(string name, Type retType, Param[] pars)
        {
            System.Diagnostics.Debug.Assert(retType != null);
            MethSig mSig = new MethSig(name);
            mSig.SetParTypes(pars);
            MethodDef meth = (MethodDef)GetMethod(mSig);
            if(meth != null)
                throw new DescriptorException("Method " + meth.NameString());
            mSig.retType = retType;
            meth = new MethodDef(this, mSig, pars);
            methods.Add(meth);
            return meth;
        }

        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="genPars">generic parameters</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameters</param>
        /// <returns>a descriptor for this new method</returns>
        public MethodDef AddMethod(string name, GenericParam[] genPars, Type retType, Param[] pars)
        {
            MethodDef meth = AddMethod(name, retType, pars);
            if((genPars != null) && (genPars.Length > 0)) {
                for(int i=0; i < genPars.Length; i++) {
                    genPars[i].SetMethParam(meth, i);
                }
                meth.SetGenericParams(genPars);
            }
            return meth;
        }

        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="mAtts">attributes for this method</param>
        /// <param name="iAtts">implementation attributes for this method</param>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameters</param>
        /// <returns>a descriptor for this new method</returns>
        public MethodDef AddMethod(MethAttr mAtts, ImplAttr iAtts, string name,
                                  Type retType, Param[] pars)
        {
            MethodDef meth = AddMethod(name, retType, pars);
            meth.AddMethAttribute(mAtts);
            meth.AddImplAttribute(iAtts);
            return meth;
        }
        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="mAtts">attributes for this method</param>
        /// <param name="iAtts">implementation attributes for this method</param>
        /// <param name="name">method name</param>
        /// <param name="genPars">generic parameters</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameters</param>
        /// <returns>a descriptor for this new method</returns>
        public MethodDef AddMethod(MethAttr mAtts, ImplAttr iAtts, string name,
                                   GenericParam[] genPars, Type retType, Param[] pars)
        {
            MethodDef meth = AddMethod(name, genPars, retType, pars);
            meth.AddMethAttribute(mAtts);
            meth.AddImplAttribute(iAtts);
            return meth;
        }

        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="meth">Descriptor for the method to be added</param>
        public void AddMethod(MethodDef meth)
        {
            MethodDef m = (MethodDef)GetMethodDesc(meth.Name(), meth.GetParTypes());
            if(m != null)
                throw new DescriptorException("Method " + m.NameString());
            methods.Add(meth);
            meth.SetParent(this);
        }

        /// <summary>
        /// Get the descriptor for the method "name" of this class
        /// </summary>
        /// <param name="name">The name of the method to be retrieved</param>
        /// <returns>The method descriptor for "name"</returns>
        public MethodDef GetMethod(string name)
        {
            return (MethodDef)GetMethodDesc(name);
        }

        /// <summary>
        /// Get the descriptor for the method called "name" with the signature "parTypes"
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="parTypes">The signature of the method</param>
        /// <returns>The method descriptor for name(parTypes)</returns>
        public MethodDef GetMethod(string name, Type[] parTypes)
        {
            return (MethodDef)GetMethodDesc(name, parTypes);
        }

        /// <summary>
        /// Get all the methods of this class called "name"
        /// </summary>
        /// <param name="name">The method name</param>
        /// <returns>List of methods called "name"</returns>
        public MethodDef[] GetMethods(string name)
        {
            ArrayList meths = GetMeths(name);
            return (MethodDef[])meths.ToArray(typeof(MethodDef));
        }

        /// <summary>
        /// Get all the methods for this class
        /// </summary>
        /// <returns>List of methods for this class</returns>
        public MethodDef[] GetMethods()
        {
            return (MethodDef[])methods.ToArray(typeof(MethodDef));
        }

        /// <summary>
        /// Add an event to this class
        /// </summary>
        /// <param name="name">event name</param>
        /// <param name="eType">event type</param>
        /// <returns>a descriptor for this new event</returns>
        public Event AddEvent(string name, Type eType)
        {
            Event e = (Event)FindFeature(name, events);
            if(e != null)
                throw new DescriptorException("Event " + e.NameString());
            e = new Event(name, eType, this);
            events.Add(e);
            return e;
        }

        /// <summary>
        /// Get the event "name" of this class
        /// </summary>
        /// <param name="name">The event name</param>
        /// <returns>The event desctiptor for "name"</returns>
        public Event GetEvent(string name)
        {
            return (Event)FindFeature(name, events);
        }

        /// <summary>
        /// Get all the events of this class
        /// </summary>
        /// <returns>List of events for this class</returns>
        public Event[] GetEvents()
        {
            return (Event[])events.ToArray(typeof(Event));
        }

        /// <summary>
        /// Remove the event "name" from this class
        /// </summary>
        /// <param name="name">The name of the event to be removed</param>
        public void RemoveEvent(string name)
        {
            Feature ev = FindFeature(name, events);
            if(ev != null)
                events.Remove(ev);
        }


        /// <summary>
        /// Add a property to this class
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="pars">parameters</param>
        /// <param name="retType">return type</param>
        /// <returns>a descriptor for this new property</returns>
        public Property AddProperty(string name, Type retType, Type[] pars)
        {
            Property p = (Property)FindFeature(name, properties);
            if(p != null)
                throw new DescriptorException("Property " + p.NameString());
            p = new Property(name, retType, pars, this);
            properties.Add(p);
            return p;
        }


        /// <summary>
        /// Get the property "name" for this class
        /// </summary>
        /// <param name="name">Descriptor for the property "name"</param>
        /// <returns></returns>
        public Property GetProperty(string name)
        {
            return (Property)FindFeature(name, properties);
        }

        /// <summary>
        /// Get all the properties for this class
        /// </summary>
        /// <returns>List of properties for this class</returns>
        public Property[] GetProperties()
        {
            return (Property[])properties.ToArray(typeof(Property));
        }

        /// <summary>
        /// Remove the property "name" from this class
        /// </summary>
        /// <param name="name">Name of the property to be removed</param>
        public void RemoveProperty(string name)
        {
            Feature prop = FindFeature(name, properties);
            if(prop != null)
                properties.Remove(prop);
        }

        /// <summary>
        /// Add a nested class to this class
        /// </summary>
        /// <param name="attrSet">attributes for this nested class</param>
        /// <param name="name">nested class name</param>
        /// <returns>a descriptor for this new nested class</returns>
        public NestedClassDef AddNestedClass(TypeAttr attrSet, string name)
        {
            NestedClassDef nClass = GetNestedClass(name);
            if(nClass != null)
                throw new DescriptorException("Nested Class " + nClass.NameString());
            nClass = new NestedClassDef(this, attrSet, name);
            nestedClasses.Add(nClass);
            return (nClass);
        }

        /// <summary>
        /// Add a nested class to this class
        /// </summary>
        /// <param name="attrSet">attributes for this nested class</param>
        /// <param name="name">nested class name</param>
        /// <param name="sType">super type of this nested class</param>
        /// <returns>a descriptor for this new nested class</returns>
        public NestedClassDef AddNestedClass(TypeAttr attrSet, string name, Class sType)
        {
            NestedClassDef nClass = AddNestedClass(attrSet, name);
            nClass.superType = sType;
            return (nClass);
        }

        /// <summary>
        /// Get the nested class called "name"
        /// </summary>
        /// <param name="name">The name of the nested class</param>
        /// <returns>Descriptor for the nested class</returns>
        public NestedClassDef GetNestedClass(string name)
        {
            //CheckNestedClassNames(MDTable.TypeDef);
            return (NestedClassDef)GetNested(name);
        }

        /// <summary>
        /// Add layout information for this class.  This class must have the
        /// sequential or explicit attribute.
        /// </summary>
        /// <param name="packSize">packing size (.pack)</param>
        /// <param name="classSize">class size (.size)</param>
        public void AddLayoutInfo(int packSize, int classSize)
        {
            layout = new ClassLayout(packSize, classSize, this);
        }

        /// <summary>
        /// Get the pack size for this class (only valid for ExplicitLayout or SequentialLayout
        /// </summary>
        /// <returns>Class pack size</returns>
        public int GetPackSize()
        {
            if((layout == null) && (((flags & (uint)TypeAttr.ExplicitLayout) != 0) || 
        ((flags & (uint)TypeAttr.SequentialLayout) != 0)) && (buffer != null)) {
                buffer.SetElementPosition(MDTable.ClassLayout, 0);
                //layout = buffer.FindParent(MDTable.ClassLayout,this);
            }
            if(layout != null)
                return layout.GetPack();
            return 0;
        }

        /// <summary>
        /// Get the size of this class (only valid for ExplicitLayout or SequentialLayout
        /// </summary>
        /// <returns>The size of this class</returns>
        public int GetClassSize()
        {
            if(layout == null)
                return 0;
            return layout.GetSize();
        }

        /// <summary>
        /// Get the ClassRef for this ClassDef, if there is one
        /// </summary>
        public ClassRef RefOf
        {
            get
            {
                if(refOf == null) {
                    ModuleRef modRef = scope.refOf;
                    if(modRef != null)
                        refOf = modRef.GetClass(name);
                }
                return refOf;
            }
        }

        /// <summary>
        /// Make a ClassRef for this ClassDef
        /// </summary>
        /// <returns>ClassRef equivalent to this ClassDef</returns>
        public ClassRef MakeRefOf()
        {
            if(refOf == null) {
                Assembly assem = scope.GetThisAssembly();
                ReferenceScope scopeRef;
                if(assem != null)
                    scopeRef = assem.MakeRefOf();
                else
                    scopeRef = scope.MakeRefOf();

                refOf = scopeRef.GetClass(name);
                if(refOf == null) {
                    refOf = new ClassRef(scopeRef, nameSpace, name);
                    scopeRef.AddToClassList(refOf);
                }
                refOf.defOf = this;
            }
            return refOf;
        }

        /// <summary>
        /// Use a method as the implementation for another method (.override)
        /// </summary>
        /// <param name="decl">the method to be overridden</param>
        /// <param name="body">the implementation to be used</param>
        public void AddMethodOverride(Method decl, Method body)
        {
            methodImpls.Add(new MethodImpl(this, decl, body));
        }

        public void AddMethodOverride(MethodImpl mImpl)
        {
            methodImpls.Add(mImpl);
            mImpl.SetOwner(this);
        }

        public MethodImpl[] GetMethodOverrides()
        {
            return (MethodImpl[])methodImpls.ToArray(typeof(MethodImpl));
        }

        public void RemoveMethodOverride(MethodImpl mImpl)
        {
            if(methodImpls != null)
                methodImpls.Remove(mImpl);
        }


        /// <summary>
        /// Add security to this class
        /// </summary>
        /// <param name="act">The security action</param>
        /// <param name="permissionSet">Permission set</param>
        public void AddSecurity(SecurityAction act, byte[] permissionSet)
        {
            AddSecurity(new DeclSecurity(this, act, permissionSet));
            // securityActions = permissionSet;
        }

        /// <summary>
        /// Add security to this class
        /// </summary>
        /// <param name="sec">The descriptor for the security to add to this class</param>
        internal void AddSecurity(DeclSecurity sec)
        {
            flags |= HasSecurity;
            if(security == null)
                security = new ArrayList();
            security.Add(sec);
        }

        /// <summary>
        /// Get the security descriptor associated with this class
        /// </summary>
        /// <returns></returns>
        public DeclSecurity[] GetSecurity()
        {
            if(security == null)
                return null;
            return (DeclSecurity[])security.ToArray(typeof(DeclSecurity));
        }

        /// <summary>
        /// Remove the security associated with this class
        /// </summary>
        public void DeleteSecurity()
        {
            flags &= NoSecurity;
            security = null;
        }

        //public void AddLineInfo(int row, int col) { }

        /*----------------------------- internal functions ------------------------------*/

        internal bool isPublic()
        {
            uint vis = flags & VisibilityMask;
            return (vis > 0) && (vis != 3) && (vis != 5);
        }

        internal static bool isPublic(uint flags)
        {
            uint vis = flags & VisibilityMask;
            return (vis > 0) && (vis != 3) && (vis != 5);
        }

        internal static bool isNested(uint flags)
        {
            uint vis = flags & VisibilityMask;
            return vis > 1;
        }

        internal override bool isDef()
        {
            return true;
        }

        private Feature FindFeature(string name, ArrayList featureList)
        {
            if(featureList == null)
                return null;
            for(int i=0; i < featureList.Count; i++) {
                if(((Feature)featureList[i]).Name() == name) {
                    return (Feature)featureList[i];
                }
            }
            return null;
        }

        private int FindGenericParam(string name)
        {
            for(int i=0; i < genericParams.Count; i++) {
                GenericParam gp = (GenericParam)genericParams[i];
                if(gp.GetName() == name)
                    return i;
            }
            return -1;
        }

        internal ClassLayout Layout
        {
            set
            {
                layout = value;
            }
            get
            {
                return layout;
            }
        }

        internal void SetScope(PEFile mod)
        {
            scope = mod;
        }

        internal void AddImplementedInterface(InterfaceImpl iImpl)
        {
            interfaces.Add(iImpl);
        }

        internal NestedClassDef MakeNestedClass(ClassDef parent)
        {
            NestedClassDef nClass = new NestedClassDef(parent, (TypeAttr)flags, name);
            ClassDef tmp = nClass;
            tmp.fieldIx = fieldIx;
            tmp.fieldEndIx = fieldEndIx;
            tmp.methodIx = methodIx;
            tmp.methodEndIx = methodEndIx;
            tmp.extendsIx = extendsIx;
            tmp.Row = Row;
            parent.nestedClasses.Add(nClass);
            return nClass;
        }

        private void ReadSecurity()
        {
            //if ((security == null) && ((flags & HasSecurity) != 0) && (buffer != null)) 
            //security = buffer.FindParent(MDTable.DeclSecurity,this);
        }

        public override void MakeSpecial()
        {
            special = true;
            superType = null;
            flags = (uint)TypeAttr.Private;
        }

        internal void AddMethodImpl(MethodImpl impl)
        {
            methodImpls.Add(impl);
        }

        internal void AddEvent(Event ev)
        {
            if(ev == null)
                return;
            ev.SetParent(this);
            events.Add(ev);
        }

        internal void AddProperty(Property prop)
        {
            if(prop == null)
                return;
            prop.SetParent(this);
            properties.Add(prop);
        }

        internal void AddToFeatureList(ArrayList list, MDTable tabIx)
        {
            if(tabIx == MDTable.Event) {
                events.AddRange(list);
            } else {
                properties.AddRange(list);
            }
        }

        // fix for Whidbey bug
        internal void AddGenericsToTable(MetaDataOut md)
        {
            //for (int i=0; i < methods.Count; i++) {
            //  ((MethodDef)methods[i]).AddGenericsToTable(md);
            //}
            for(int i=0; i < genericParams.Count; i++) {
                md.AddToTable(MDTable.GenericParam, (GenericParam)genericParams[i]);
            }
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            md.AddToTable(MDTable.TypeDef, this);
            //if ((flags & (uint)TypeAttr.Interface) != 0) { superType = null; }
            if(superType != null) {
                superType.BuildMDTables(md);
                if(superType is ClassSpec)
                    md.AddToTable(MDTable.TypeSpec, superType);
            }
            for(int i=0; i < genericParams.Count; i++) {
                ((GenericParam)genericParams[i]).BuildMDTables(md);
            }
            nameIx = md.AddToStringsHeap(name);
            nameSpaceIx = md.AddToStringsHeap(nameSpace);
            if(security != null) {
                for(int i=0; i < security.Count; i++) {
                    ((DeclSecurity)security[i]).BuildMDTables(md);
                }
            }
            // Console.WriteLine("Building tables for " + name);
            if(layout != null)
                layout.BuildMDTables(md);
            // Console.WriteLine("adding methods " + methods.Count);
            methodIx = md.TableIndex(MDTable.Method);
            for(int i=0; i < methods.Count; i++) {
                ((MethodDef)methods[i]).BuildMDTables(md);
            }
            // Console.WriteLine("adding fields");
            fieldIx = md.TableIndex(MDTable.Field);
            for(int i=0; i < fields.Count; i++) {
                ((FieldDef)fields[i]).BuildMDTables(md);
            }
            // Console.WriteLine("adding interfaceimpls and methodimpls");
            if(interfaces.Count > 0) {
                for(int i=0; i < interfaces.Count; i++) {
                    ((InterfaceImpl)interfaces[i]).BuildMDTables(md);
                }
            }
            if(methodImpls.Count > 0) {
                for(int i=0; i < methodImpls.Count; i++) {
                    ((MethodImpl)methodImpls[i]).BuildMDTables(md);
                }
            }
            // Console.WriteLine("adding events and properties");
            if(events.Count > 0) {
                new MapElem(this, md.TableIndex(MDTable.Event), MDTable.EventMap).BuildMDTables(md);
                for(int i=0; i < events.Count; i++) {
                    ((Event)events[i]).BuildMDTables(md);
                }
            }
            if(properties.Count > 0) {
                new MapElem(this, md.TableIndex(MDTable.Property), MDTable.PropertyMap).BuildMDTables(md);
                for(int i=0; i < properties.Count; i++) {
                    ((Property)properties[i]).BuildMDTables(md);
                }
            }
            // Console.WriteLine("Adding nested classes");
            if(nestedClasses.Count > 0) {
                for(int i=0; i < nestedClasses.Count; i++) {
                    ClassDef nClass = (ClassDef)nestedClasses[i];
                    nClass.BuildMDTables(md);
                    new MapElem(nClass, this, MDTable.NestedClass).BuildTables(md);
                }
            }
            // Console.WriteLine("End of building tables");
        }

        internal static uint Size(MetaData md)
        {
            return 4 + 2 * md.StringsIndexSize() + 
        md.CodedIndexSize(CIx.TypeDefOrRef) +
        md.TableIndexSize(MDTable.Field) + 
        md.TableIndexSize(MDTable.Method);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write(flags);
            output.StringsIndex(nameIx);
            output.StringsIndex(nameSpaceIx);
            //if (superType != null) 
            // Console.WriteLine("getting coded index for superType of " + name + " = " + superType.GetCodedIx(CIx.TypeDefOrRef));
            output.WriteCodedIndex(CIx.TypeDefOrRef, superType);
            output.WriteIndex(MDTable.Field, fieldIx);
            output.WriteIndex(MDTable.Method, methodIx);
        }

        internal sealed override uint TypeDefOrRefToken()
        {
            uint cIx = Row;
            cIx = cIx << 2;
            return cIx;
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.TypeDefOrRef):
                return 0;
            case (CIx.HasCustomAttr):
                return 3;
            case (CIx.HasDeclSecurity):
                return 0;
            case (CIx.TypeOrMethodDef):
                return 0;
            }
            return 0;
        }

        internal override string ClassName()
        {
            return (nameSpace + "." + name);
        }

        internal override string NameString()
        {
            string nameString = "";
            if(scope != null)
                nameString = "[" + scope.NameString() + "]";
            if((nameSpace != null) && (nameSpace.Length > 0))
                nameString += nameSpace + ".";
            nameString += name;
            return nameString;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a Nested Class defined in an assembly
    /// </summary>
    public class NestedClassDef : ClassDef {
        ClassDef parent;

        /*-------------------- Constructors ---------------------------------*/

        internal NestedClassDef(ClassDef parent, TypeAttr attrSet, string name)
            : base(parent.GetScope(), attrSet, "", name)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Fetch the PEFile which contains this class
        /// </summary>
        /// <returns>PEFile containing this class</returns>
        public override PEFile GetScope()
        {
            if(scope == null)
                scope = parent.GetScope();
            return scope;
        }

        /// <summary>
        /// Get the enclosing class for this nested class
        /// </summary>
        /// <returns>ClassDef of the enclosing class</returns>
        public ClassDef GetParentClass()
        {
            return parent;
        }

        internal void SetParent(ClassDef par)
        {
            parent = par;
        }

        internal override string ClassName()
        {
            string nameString = name;
            if(parent != null)
                nameString = parent.TypeName() + "+" + name;
            return nameString;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a class/interface declared in another module of THIS 
    /// assembly, or in another assembly.
    /// </summary>
    public class ClassRef : ClassDesc {
        protected ReferenceScope scope;
        protected uint resScopeIx = 0;
        internal ExternClass externClass;
        internal ClassDef defOf;
        internal bool readAsDef = false;

        /*-------------------- Constructors ---------------------------------*/

        internal ClassRef(ReferenceScope scope, string nsName, string name)
            : base(nsName, name)
        {
            this.scope = scope;
            tabIx = MDTable.TypeRef;
        }

        internal ClassRef(uint scopeIx, string nsName, string name)
            : base(nsName, name)
        {
            resScopeIx = scopeIx;
            tabIx = MDTable.TypeRef;
        }

        internal static ClassRef ReadDef(PEReader buff, ReferenceScope resScope, uint index)
        {
            uint junk = buff.ReadUInt32();
            string cName = buff.GetString();
            string nsName = buff.GetString();
            ClassRef newClass = (ClassRef)resScope.GetExistingClass(nsName, cName);
            if(newClass == null) {
                newClass = new ClassRef(resScope, nsName, cName);
                resScope.AddToClassList(newClass);
            }
            newClass.readAsDef = true;
            newClass.Row = index;
            junk = buff.GetCodedIndex(CIx.TypeDefOrRef);
            newClass.fieldIx = buff.GetIndex(MDTable.Field);
            newClass.methodIx = buff.GetIndex(MDTable.Method);
            return newClass;
        }

        internal static void Read(PEReader buff, TableRow[] typeRefs, bool resolve)
        {
            for(uint i=0; i < typeRefs.Length; i++) {
                uint resScopeIx = buff.GetCodedIndex(CIx.ResolutionScope);
                string name = buff.GetString();
                string nameSpace = buff.GetString();
                if(buff.CodedTable(CIx.ResolutionScope, resScopeIx) == MDTable.TypeRef)
                    typeRefs[i] = new NestedClassRef(resScopeIx, nameSpace, name);
                else
                    typeRefs[i] = new ClassRef(resScopeIx, nameSpace, name);
                typeRefs[i].Row = i+1;
            }
            if(resolve) {
                for(int i=0; i < typeRefs.Length; i++) {
                    ((ClassRef)typeRefs[i]).ResolveParent(buff, false);
                }
            }
        }

        internal static ClassRef ReadClass(PEReader buff, ReferenceScope resScope)
        {
            uint resScopeIx = buff.GetCodedIndex(CIx.ResolutionScope);
            string name = buff.GetString();
            string nameSpace = buff.GetString();
            ClassRef newClass = (ClassRef)resScope.GetExistingClass(nameSpace, name);
            if(newClass == null)
                newClass = new ClassRef(resScope, nameSpace, name);
            return newClass;
        }

        internal virtual void ResolveParent(PEReader buff, bool isExtern)
        {
            CIx cIx = CIx.ResolutionScope;
            if(isExtern)
                cIx = CIx.Implementation;
            if(scope != null)
                return;
            MetaDataElement parentScope = buff.GetCodedElement(cIx, resScopeIx);
            if(parentScope is Module) {  // special code for glitch in Everett ilasm
                ClassDef newDef = new ClassDef((PEFile)parentScope, 0, nameSpace, name);
                ((Module)parentScope).AddToClassList(newDef);
                buff.InsertInTable(MDTable.TypeRef, Row, newDef);
            } else {
                scope = (ReferenceScope)buff.GetCodedElement(cIx, resScopeIx);
                ClassRef existing = (ClassRef)scope.GetExistingClass(nameSpace, name);
                if(existing == null) {
                    scope.AddToClassList(this);
                } else {
                    if(isExtern)
                        buff.InsertInTable(MDTable.ExportedType, Row, existing);
                    else
                        buff.InsertInTable(MDTable.TypeRef, Row, existing);
                }
            }
        }

        /*------------------------- public set and get methods --------------------------*/

        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameter types</param>
        /// <returns>a descriptor for this method</returns>
        public MethodRef AddMethod(string name, Type retType, Type[] pars)
        {
            System.Diagnostics.Debug.Assert(retType != null);
            MethodRef meth = (MethodRef)GetMethodDesc(name, pars);
            if(meth != null)
                DescriptorError(meth);
            meth = new MethodRef(this, name, retType, pars);
            methods.Add(meth);
            return meth;
        }

        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="genPars">generic parameters</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameter types</param>
        /// <returns>a descriptor for this method</returns>
        public MethodRef AddMethod(string name, GenericParam[] genPars, Type retType, Type[] pars)
        {
            MethodRef meth = AddMethod(name, retType, pars);
            if((genPars != null) && (genPars.Length > 0)) {
                for(int i=0; i < genPars.Length; i++) {
                    genPars[i].SetMethParam(meth, i);
                }
                meth.SetGenericParams(genPars);
            }
            return meth;
        }

        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameter types</param>
        /// <returns>a descriptor for this method</returns>
        public MethodRef AddVarArgMethod(string name, Type retType, Type[] pars, Type[] optPars)
        {
            MethodRef meth = AddMethod(name, retType, pars);
            meth.MakeVarArgMethod(null, optPars);
            return meth;
        }
        /// <summary>
        /// Add a method to this class
        /// </summary>
        /// <param name="name">method name</param>
        /// <param name="retType">return type</param>
        /// <param name="pars">parameter types</param>
        /// <returns>a descriptor for this method</returns>
        public MethodRef AddVarArgMethod(string name, GenericParam[] genPars, Type retType, Type[] pars, Type[] optPars)
        {
            MethodRef meth = AddMethod(name, genPars, retType, pars);
            meth.MakeVarArgMethod(null, optPars);
            return meth;
        }

        /// <summary>
        /// Get the method "name" for this class
        /// </summary>
        /// <param name="name">The method name</param>
        /// <returns>Descriptor for the method "name" for this class</returns>
        public MethodRef GetMethod(string name)
        {
            return (MethodRef)GetMethodDesc(name);
        }

        /// <summary>
        /// Get the method "name(parTypes)" for this class
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="parTypes">Method signature</param>
        /// <returns>Descriptor for "name(parTypes)"</returns>
        public MethodRef GetMethod(string name, Type[] parTypes)
        {
            return (MethodRef)GetMethodDesc(name, parTypes);
        }

        /// <summary>
        /// Get the vararg method "name(parTypes,optTypes)" for this class
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="parTypes">Method parameter types</param>
        /// <param name="optTypes">Optional parameter types</param>
        /// <returns>Descriptor for "name(parTypes,optTypes)"</returns>
        public MethodRef GetMethod(string name, Type[] parTypes, Type[] optTypes)
        {
            return (MethodRef)GetMethodDesc(name, parTypes, optTypes);
        }

        /// <summary>
        /// Get the descriptors for the all methods name "name" for this class
        /// </summary>
        /// <param name="name">Method name</param>
        /// <returns>List of methods called "name"</returns>
        public MethodRef[] GetMethods(string name)
        {
            ArrayList meths = GetMeths(name);
            return (MethodRef[])meths.ToArray(typeof(MethodRef));
        }


        /// <summary>
        /// Get all the methods for this class
        /// </summary>
        /// <returns>List of methods for this class</returns>
        public MethodRef[] GetMethods()
        {
            return (MethodRef[])methods.ToArray(typeof(MethodRef));
        }

        /// <summary>
        /// Add a field to this class
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="fType">field type</param>
        /// <returns>a descriptor for this field</returns>
        public FieldRef AddField(string name, Type fType)
        {
            FieldRef fld = (FieldRef)FindField(name);
            if(fld != null)
                DescriptorError(fld);
            fld = new FieldRef(this, name, fType);
            fields.Add(fld);
            return fld;
        }

        /// <summary>
        /// Get the descriptor for the field "name" for this class
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Descriptor for field "name"</returns>
        public FieldRef GetField(string name)
        {
            return (FieldRef)FindField(name);
        }

        /// <summary>
        /// Get all the fields for this class
        /// </summary>
        /// <returns>List of fields for this class</returns>
        public FieldRef[] GetFields()
        {
            return (FieldRef[])fields.ToArray(typeof(FieldRef));
        }

        /// <summary>
        /// Add a nested class to this class
        /// </summary>
        /// <param name="name">Nested class name</param>
        /// <returns>Descriptor for the nested class "name"</returns>
        public NestedClassRef AddNestedClass(string name)
        {
            NestedClassRef nestedClass = (NestedClassRef)GetNested(name);
            if(nestedClass != null)
                DescriptorError(nestedClass);
            nestedClass = new NestedClassRef(this, name);
            AddToClassList(nestedClass);
            return nestedClass;
        }

        /// <summary>
        /// Get the nested class "name"
        /// </summary>
        /// <param name="name">Nestec class name</param>
        /// <returns>Descriptor for the nested class "name"</returns>
        public NestedClassRef GetNestedClass(string name)
        {
            // check nested names
            return (NestedClassRef)GetNested(name);
        }

        /// <summary>
        /// Make this Class exported from an Assembly (ie. add to ExportedType table)
        /// </summary>
        public void MakeExported()
        {
            if((scope == null) || (!(scope is ModuleRef)))
                throw new Exception("Module not set for class to be exported");
            ((ModuleRef)scope).AddToExportedClassList(this);
        }

        /// <summary>
        /// Get the scope or "parent" of this ClassRef (either ModuleRef or AssemblyRef)
        /// </summary>
        /// <returns>Descriptor for the scope containing this class</returns>
        public virtual ReferenceScope GetScope()
        {
            return scope;
        }

        public override MetaDataElement GetParent()
        {
            return scope;
        }

        /*----------------------------- internal functions ------------------------------*/

        internal void SetExternClass(ExternClass eClass)
        {
            externClass = eClass;
        }

        internal void SetScope(ReferenceScope scope)
        {
            this.scope = scope;
        }

        internal void AddField(FieldRef fld)
        {
            fields.Add(fld);
            fld.SetParent(this);
        }

        internal void AddMethod(MethodRef meth)
        {
            MethodRef m = (MethodRef)GetMethodDesc(meth.Name(), meth.GetParTypes());
            if(m == null) {
                methods.Add(meth);
                meth.SetParent(this);
            }
        }

        /*
        internal FieldRef GetExistingField(string fName, uint tyIx, PEReader buff) {
          FieldRef existing = (FieldRef)FindField(fName);
          if (existing != null) {
            Type fType = buff.GetBlobType(tyIx);
            if (!fType.SameType(existing.GetFieldType()))
              throw new DescriptorException("Cannot have two fields (" + fName +
                ") for class " + name);
          }
          return existing;
        }
        */

        /*
        internal MethodRef CheckForMethod(string mName, uint sigIx, PEReader buff) {
          int exIx = FindMeth(mName,0);
          if (exIx > -1) {
            MethSig mType = buff.ReadMethSig(sigIx);
            mType.name = mName;
            exIx = FindMeth(mType,0);
            if (exIx > -1) 
              return (MethodRef)methods[exIx];
          }
          return null;
        }
        */

        internal override string ClassName()
        {
            string nameString = nameSpace + "." + name;
            if((scope != null) && (scope is AssemblyRef))
                nameString += (", " + ((AssemblyRef)scope).AssemblyString());
            return nameString;
        }

        internal bool HasParent(uint tok)
        {
            return resScopeIx == tok;
        }

        internal override void BuildTables(MetaDataOut md)
        {
            if(!special) {
                md.AddToTable(MDTable.TypeRef, this);
                nameIx = md.AddToStringsHeap(name);
                nameSpaceIx = md.AddToStringsHeap(nameSpace);
            }
            scope.BuildMDTables(md);
        }

        internal static uint Size(MetaData md)
        {
            return md.CodedIndexSize(CIx.ResolutionScope) + 2 * md.StringsIndexSize();
        }

        internal override void Write(PEWriter output)
        {
            output.WriteCodedIndex(CIx.ResolutionScope, scope);
            output.StringsIndex(nameIx);
            output.StringsIndex(nameSpaceIx);
        }

        internal override sealed uint TypeDefOrRefToken()
        {
            uint cIx = Row;
            cIx = (cIx << 2) | 0x1;
            return cIx;
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.TypeDefOrRef):
                return 1;
            case (CIx.HasCustomAttr):
                return 2;
            case (CIx.MemberRefParent):
                return 1;
            case (CIx.ResolutionScope):
                return 3;
            }
            return 0;
        }

        internal override string NameString()
        {
            string nameString = "";
            if(scope != null)
                nameString = "[" + scope.NameString() + "]";
            if((nameSpace != null) && (nameSpace.Length > 0))
                nameString += nameSpace + ".";
            nameString += name;
            return nameString;
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a reference to a Nested Class
    /// </summary>
    public class NestedClassRef : ClassRef {
        ClassRef parent;
        internal uint parentIx = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal NestedClassRef(ClassRef parent, string name)
            : base(parent.GetScope(), "", name)
        {
            this.parent = parent;
        }

        internal NestedClassRef(uint scopeIx, string nsName, string name)
            : base(scopeIx, nsName, name)
        {
        }

        internal NestedClassRef(ReferenceScope scope, string nsName, string name)
            : base(scope, nsName, name)
        {
        }

        internal override void ResolveParent(PEReader buff, bool isExtern)
        {
            if(parent != null)
                return;
            CIx cIx = CIx.ResolutionScope;
            if(isExtern)
                cIx = CIx.Implementation;
            parent = (ClassRef)buff.GetCodedElement(cIx, resScopeIx);
            parent.ResolveParent(buff, isExtern);
            parent = (ClassRef)buff.GetCodedElement(cIx, resScopeIx);
            if(parent == null)
                return;
            NestedClassRef existing = parent.GetNestedClass(name);
            if(existing == null) {
                scope = parent.GetScope();
                parent.AddToClassList(this);
            } else if(isExtern)
                buff.InsertInTable(MDTable.ExportedType, Row, existing);
            else
                buff.InsertInTable(MDTable.TypeRef, Row, existing);
        }

        /// <summary>
        /// Get the scope of this ClassRef (either ModuleRef or AssemblyRef)
        /// </summary>
        /// <returns>Descriptor for the scope containing this class</returns>
        public override ReferenceScope GetScope()
        {
            if(scope == null)
                scope = parent.GetScope();
            return scope;
        }

        /// <summary>
        /// Get the parent (enclosing ClassRef) for this nested class
        /// </summary>
        /// <returns>Enclosing class descriptor</returns>
        public ClassRef GetParentClass()
        {
            return parent;
        }

        internal void SetParent(ClassRef paren)
        {
            parent = paren;
        }

        internal override string ClassName()
        {
            string nameString = name;
            if(parent != null)
                nameString = parent.TypeName() + "+" + name;
            if((scope != null) && (scope is AssemblyRef))
                nameString += (", " + ((AssemblyRef)scope).AssemblyString());
            return nameString;
        }

        internal override string NameString()
        {
            if(parent == null)
                return name;
            return parent.NameString() + "+" + name;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(!special) {
                md.AddToTable(MDTable.TypeRef, this);
                nameIx = md.AddToStringsHeap(name);
                nameSpaceIx = md.AddToStringsHeap(nameSpace);
            }
            parent.BuildMDTables(md);
        }

        internal sealed override void Write(PEWriter output)
        {
            output.WriteCodedIndex(CIx.ResolutionScope, parent);
            output.StringsIndex(nameIx);
            output.StringsIndex(nameSpaceIx);
        }


    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a class defined in System (mscorlib)
    /// </summary>
    internal class SystemClass : ClassRef {
        PrimitiveType elemType;
        internal bool added = false;

        internal SystemClass(AssemblyRef paren, PrimitiveType eType)
            : base(paren, "System", eType.GetName())
        {
            elemType = eType;
        }

        //   internal override sealed void AddTypeSpec(MetaDataOut md) {
        //     elemType.AddTypeSpec(md);
        //      if (typeSpec == null) typeSpec = (TypeSpec)elemType.GetTypeSpec(md);
        //      return typeSpec;
        //   }

        internal sealed override void TypeSig(MemoryStream str)
        {
            str.WriteByte(elemType.GetTypeIndex());
        }

        internal override bool SameType(Type tstType)
        {
            if(tstType is SystemClass)
                return elemType == ((SystemClass)tstType).elemType;
            return elemType == tstType;
        }



    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a custom modifier of a type (modopt or modreq)
    /// </summary>
    public class CustomModifiedType : Type {
        Type type;
        Class cmodType;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new custom modifier for a type
        /// </summary>
        /// <param name="type">the type to be modified</param>
        /// <param name="cmod">the modifier</param>
        /// <param name="cmodType">the type reference to be associated with the type</param>
        public CustomModifiedType(Type type, CustomModifier cmod, Class cmodType)
            : base((byte)cmod)
        {
            this.type = type;
            this.cmodType = cmodType;
        }

        /*------------------------- public set and get methods --------------------------*/

        public void SetModifiedType(Type modType)
        {
            type = modType;
        }
        public Type GetModifiedType()
        {
            return type;
        }

        public void SetModifingType(Class mod)
        {
            cmodType = mod;
        }
        public Class GetModifingType()
        {
            return cmodType;
        }

        public void SetModifier(CustomModifier cmod)
        {
            typeIndex = (byte)cmod;
        }
        public CustomModifier GetModifier()
        {
            return (CustomModifier)typeIndex;
        }

        /*----------------------------- internal functions ------------------------------*/

        internal override bool SameType(Type tstType)
        {
            if(this == tstType)
                return true;
            if(tstType is CustomModifiedType) {
                CustomModifiedType cmTstType = (CustomModifiedType)tstType;
                return type.SameType(cmTstType.type) && 
          cmodType.SameType(cmTstType.cmodType);
            }
            return false;
        }
        internal sealed override void TypeSig(MemoryStream str)
        {
            str.WriteByte(typeIndex);
            MetaDataOut.CompressNum(cmodType.TypeDefOrRefToken(), str);
            type.TypeSig(str);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(!(cmodType is ClassDef))
                cmodType.BuildMDTables(md);
            if(!(type is ClassDef))
                type.BuildMDTables(md);
        }

    }
    /**************************************************************************/
    internal class Pinned : Type {
        internal Pinned()
            : base((byte)ElementType.Pinned)
        {
        }
    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for the Primitive types defined in IL
    /// </summary>
    public class PrimitiveType : TypeSpec {
        private string name;
        private int systemTypeIndex;
        internal static int NumSystemTypes = 18;

        public static readonly PrimitiveType Void = new PrimitiveType(0x01, "Void", 0);
        public static readonly PrimitiveType Boolean = new PrimitiveType(0x02, "Boolean", 1);
        public static readonly PrimitiveType Char = new PrimitiveType(0x03, "Char", 2);
        public static readonly PrimitiveType Int8 = new PrimitiveType(0x04, "SByte", 3);
        public static readonly PrimitiveType UInt8 = new PrimitiveType(0x05, "Byte", 4);
        public static readonly PrimitiveType Int16 = new PrimitiveType(0x06, "Int16", 5);
        public static readonly PrimitiveType UInt16 = new PrimitiveType(0x07, "UInt16", 6);
        public static readonly PrimitiveType Int32 = new PrimitiveType(0x08, "Int32", 7);
        public static readonly PrimitiveType UInt32 = new PrimitiveType(0x09, "UInt32", 8);
        public static readonly PrimitiveType Int64 = new PrimitiveType(0x0A, "Int64", 9);
        public static readonly PrimitiveType UInt64 = new PrimitiveType(0x0B, "UInt64", 10);
        public static readonly PrimitiveType Float32 = new PrimitiveType(0x0C, "Single", 11);
        public static readonly PrimitiveType Float64 = new PrimitiveType(0x0D, "Double", 12);
        public static readonly PrimitiveType String = new PrimitiveType(0x0E, "String", 13);
        internal static readonly PrimitiveType Class = new PrimitiveType(0x12);
        public static readonly PrimitiveType TypedRef = new PrimitiveType(0x16, "TypedReference", 14);
        public static readonly PrimitiveType IntPtr = new PrimitiveType(0x18, "IntPtr", 15);
        public static readonly PrimitiveType UIntPtr = new PrimitiveType(0x19, "UIntPtr", 16);
        public static readonly PrimitiveType Object = new PrimitiveType(0x1C, "Object", 17);
        internal static readonly PrimitiveType ClassType = new PrimitiveType(0x50);
        internal static readonly PrimitiveType SZArray = new PrimitiveType(0x1D);
        public static readonly PrimitiveType NativeInt = IntPtr;
        public static readonly PrimitiveType NativeUInt = UIntPtr;
        internal static PrimitiveType[] primitives = {null,Void,Boolean,Char,Int8,UInt8,
                                                   Int16,UInt16,Int32,UInt32,Int64,
                                                   UInt64,Float32,Float64,String};

        /*-------------------- Constructors ---------------------------------*/

        internal PrimitiveType(byte typeIx)
            : base(typeIx)
        {
        }

        internal PrimitiveType(byte typeIx, string name, int STIx)
            : base(typeIx)
        {
            this.name = name;
            this.systemTypeIndex = STIx;
        }

        internal string GetName()
        {
            return name;
        }

        public override string TypeName()
        {
            if(typeIndex == 0x0E)
                return "System.String";
            return name;
        }


        internal int GetSystemTypeIx()
        {
            return systemTypeIndex;
        }

        internal sealed override void TypeSig(MemoryStream str)
        {
            str.WriteByte(typeIndex);
        }

        internal sealed override bool SameType(Type tstType)
        {
            if(tstType is SystemClass)
                return tstType.SameType(this);
            return this == tstType;
        }

        internal static void ClearAddedFlags()
        {   // KJG 18-April-2005
            for(int i = 0; i < primitives.Length; i++) {
                if(primitives[i] != null)
                    primitives[i].typeSpecAdded = false;
            }
        }
    }

    /**************************************************************************/
    internal class Sentinel : Type {
        internal Sentinel()
            : base((byte)ElementType.Sentinel)
        {
        }
    }
    /**************************************************************************/
    public abstract class TypeSpec : Type {

        uint sigIx = 0;
        protected bool typeSpecAdded = false; // protected so ClearAddedFlags() can access it.

        /*-------------------- Constructors ---------------------------------*/

        internal TypeSpec(byte typeIx)
            : base(typeIx)
        {
            tabIx = MDTable.TypeSpec;
        }

        internal static void Read(PEReader buff, TableRow[] specs)
        {
            for(int i=0; i < specs.Length; i++) {
                specs[i] = new UnresolvedTypeSpec(buff, i);
                //specs[i] = buff.GetBlobType(null,null,buff.GetBlobIx());
                //if (specs[i] is GenericParam) {
                //  Console.WriteLine("GenericParam in TypeSpec table at pos " + i);
                //}
            }
        }

        internal override sealed Type AddTypeSpec(MetaDataOut md)
        {
            if(typeSpecAdded)
                return this;
            md.AddToTable(MDTable.TypeSpec, this);
            BuildMDTables(md);
            typeSpecAdded = true;
            return this;
        }

        internal override void BuildSignatures(MetaDataOut md)
        {
            MemoryStream str = new MemoryStream();
            TypeSig(str);
            sigIx = md.AddToBlobHeap(str.ToArray());
            done = false;
        }

        internal static uint Size(MetaData md)
        {
            return md.BlobIndexSize();
        }

        internal sealed override void Write(PEWriter output)
        {
            //Console.WriteLine("Writing the blob index for a TypeSpec");
            output.BlobIndex(sigIx);
        }

        internal sealed override uint GetCodedIx(CIx code)
        {
            switch(code) {
            case (CIx.TypeDefOrRef):
                return 2;
            case (CIx.HasCustomAttr):
                return 13;
            case (CIx.MemberRefParent):
                return 4;
            }
            return 0;
        }
    }
    /**************************************************************************/

    internal class UnresolvedTypeSpec : TypeSpec {
        uint blobIx;

        internal UnresolvedTypeSpec(PEReader buff, int i)
            : base(0)
        {
            blobIx = buff.GetBlobIx();
            Row = (uint)i+1;
            this.unresolved = true;
        }

        internal override void Resolve(PEReader buff)
        {
            buff.InsertInTable(MDTable.TypeSpec, Row, buff.GetBlobType(blobIx));
            this.unresolved = false;
        }


    }

    /**************************************************************************/
    /// <summary>
    /// 
    /// </summary> 
    public class GenericParTypeSpec : TypeSpec {
        GenericParam gPar;
        bool isClassPar;
        uint index;

        internal GenericParTypeSpec(GenericParam gPar)
            : base(gPar.GetTypeIndex())
        {
            this.gPar = gPar;
        }

        internal GenericParTypeSpec(int gpTypeIx, uint ix)
            : base((byte)gpTypeIx)
        {
            isClassPar = gpTypeIx == (int)ElementType.Var;
            index = ix;
        }

        internal GenericParam GetGenericParam(MethodDef meth)
        {
            if(gPar == null) {
                if(isClassPar) {
                    ClassDef methClass = (ClassDef)meth.GetParent();
                    gPar = methClass.GetGenericParam((int)index);
                } else {
                    gPar = meth.GetGenericParam((int)index);
                }
            }
            return gPar;
        }

        internal override void TypeSig(MemoryStream str)
        {
            gPar.TypeSig(str);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// The IL Array type
    /// </summary>
    public abstract class Array : TypeSpec {
        /// <summary>
        /// The element type of the array
        /// </summary>
        protected Type elemType;

        /*-------------------- Constructors ---------------------------------*/

        internal Array(Type eType, byte TypeId)
            : base(TypeId)
        {
            elemType = eType;
            tabIx = MDTable.TypeSpec;
        }

        public Type ElemType()
        {
            return elemType;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(!(elemType is ClassDef))
                elemType.BuildMDTables(md);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Multi dimensional array with explicit bounds
    /// </summary>
    public class BoundArray : Array {
        int[] lowerBounds;
        int[] sizes;
        uint numDims;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new multi dimensional array type 
        /// eg. elemType[1..5,3..10,5,,] would be 
        /// new BoundArray(elemType,5,[1,3,0],[5,10,4])
        /// </summary>
        /// <param name="elementType">the type of the elements</param>
        /// <param name="dimensions">the number of dimensions</param>
        /// <param name="loBounds">lower bounds of dimensions</param>
        /// <param name="upBounds">upper bounds of dimensions</param>
        public BoundArray(Type elementType, int dimensions, int[] loBounds,
          int[] upBounds)
            : base(elementType, 0x14)
        {
            numDims = (uint)dimensions;
            lowerBounds = loBounds;
            sizes = new int[loBounds.Length];
            if(upBounds != null) {
                for(int i=0; i < loBounds.Length; i++) {
                    sizes[i] = upBounds[i] - loBounds[i] + 1;
                }
            }
        }

        /// <summary>
        /// Create a new multi dimensional array type 
        /// eg. elemType[5,10,20] would be new BoundArray(elemType,3,[5,10,20])
        /// </summary>
        /// <param name="elementType">the type of the elements</param>
        /// <param name="dimensions">the number of dimensions</param>
        /// <param name="size">the sizes of the dimensions</param>
        public BoundArray(Type elementType, int dimensions, int[] size)
            : base(elementType, 0x14)
        {
            numDims = (uint)dimensions;
            sizes = size;
        }

        /// <summary>
        /// Create a new multi dimensional array type 
        /// eg. elemType[,,] would be new BoundArray(elemType,3)
        /// </summary>
        /// <param name="elementType">the type of the elements</param>
        /// <param name="dimensions">the number of dimensions</param>
        public BoundArray(Type elementType, int dimensions)
            : base(elementType, 0x14)
        {
            numDims = (uint)dimensions;
        }

        internal override bool SameType(Type tstType)
        {
            if(this == tstType)
                return true;
            if(!(tstType is BoundArray))
                return false;
            BoundArray bArray = (BoundArray)tstType;
            if(elemType == bArray.ElemType())
                return SameBounds(numDims, lowerBounds, sizes);
            return false;
        }

        internal bool SameBounds(uint dims, int[] lbounds, int[] sizs)
        {
            if(dims != numDims)
                return false;
            if(lowerBounds != null) {
                if((lbounds == null) || (lowerBounds.Length != lbounds.Length))
                    return false;
                for(int i=0; i < lowerBounds.Length; i++)
                    if(lowerBounds[i] != lbounds[i])
                        return false;
            } else
                if(lbounds != null)
                    return false;
            if(sizes != null) {
                if((sizs == null) || (sizes.Length != sizs.Length))
                    return false;
                for(int i=0; i < sizes.Length; i++)
                    if(sizes[i] != sizs[i])
                        return false;
            } else
                if(sizs != null)
                    return false;
            return true;
        }

        internal sealed override void TypeSig(MemoryStream str)
        {
            str.WriteByte(typeIndex);
            elemType.TypeSig(str);
            MetaDataOut.CompressNum(numDims, str);
            if((sizes != null) && (sizes.Length > 0)) {
                MetaDataOut.CompressNum((uint)sizes.Length, str);
                for(int i=0; i < sizes.Length; i++) {
                    MetaDataOut.CompressNum((uint)sizes[i], str);
                }
            } else
                str.WriteByte(0);
            if((lowerBounds != null) && (lowerBounds.Length > 0)) {
                MetaDataOut.CompressNum((uint)lowerBounds.Length, str);
                for(int i=0; i < lowerBounds.Length; i++) {
                    MetaDataOut.CompressNum((uint)lowerBounds[i], str);
                }
            } else
                str.WriteByte(0);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Single dimensional array with zero lower bound
    /// </summary>
    public class ZeroBasedArray : Array {

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new array  -   elementType[]
        /// </summary>
        /// <param name="elementType">the type of the array elements</param>
        public ZeroBasedArray(Type elementType)
            : base(elementType, (byte)ElementType.SZArray)
        {
        }

        internal sealed override void TypeSig(MemoryStream str)
        {
            str.WriteByte(typeIndex);
            elemType.TypeSig(str);
        }

        internal override bool SameType(Type tstType)
        {
            if(this == tstType)
                return true;
            if(!(tstType is ZeroBasedArray))
                return false;
            return elemType == ((ZeroBasedArray)tstType).ElemType();
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a FunctionPointer type
    /// </summary>
    /// 
    public class MethPtrType : TypeSpec {
        // MethPtrType == FNPTR
        Method meth;
        MethSig mSig;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new function pointer type
        /// </summary>
        /// <param name="meth">the function to be referenced</param>
        public MethPtrType(Method meth)
            : base((byte)ElementType.FnPtr)
        {
            this.meth = meth;
        }

        internal MethPtrType(MethSig msig)
            : base((byte)ElementType.FnPtr)
        {
            mSig = msig;
        }

        internal sealed override void TypeSig(MemoryStream str)
        {
            str.WriteByte(typeIndex);
            if(meth == null)
                mSig.TypeSig(str);
            else
                meth.TypeSig(str);
        }

        internal override bool SameType(Type tstType)
        {
            if(this == tstType)
                return true;
            if(tstType is MethPtrType) {
                MethPtrType mpType = (MethPtrType)tstType;

            }
            return false;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            Type[] types = meth.GetParTypes();
            if(types != null)
                for(int i=0; i < types.Length; i++)
                    types[i].BuildMDTables(md);
            types = meth.GetOptParTypes();
            if(types != null)
                for(int i=0; i < types.Length; i++)
                    types[i].BuildMDTables(md);
        }


        /*    internal sealed override void BuildSignatures(MetaDataOut md) {
              if (sigIx == 0) {
                MemoryStream sig = new MemoryStream();
                TypeSig(sig);
                sigIx = md.AddToBlobHeap(sig.ToArray());
              }
              done = false;
            }
            */

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for an pointer (type * or type &)
    /// </summary>
    public abstract class PtrType : TypeSpec {
        protected Type baseType;

        /*-------------------- Constructors ---------------------------------*/

        internal PtrType(Type bType, byte typeIx)
            : base(typeIx)
        {
            baseType = bType;
        }

        public Type GetBaseType()
        {
            return baseType;
        }

        internal sealed override void TypeSig(MemoryStream str)
        {
            str.WriteByte(typeIndex);
            baseType.TypeSig(str);
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(!(baseType is ClassDef))
                baseType.BuildMDTables(md);
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a managed pointer (type &  or byref)
    /// </summary>
    public class ManagedPointer : PtrType {  // <type> & (BYREF)  

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create new managed pointer to baseType
        /// </summary>
        /// <param name="bType">the base type of the pointer</param>
        public ManagedPointer(Type baseType)
            : base(baseType, 0x10)
        {
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for an unmanaged pointer (type *)
    /// </summary>
    public class UnmanagedPointer : PtrType { // PTR

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new unmanaged pointer to baseType
        /// </summary>
        /// <param name="baseType">the base type of the pointer</param>
        public UnmanagedPointer(Type baseType)
            : base(baseType, 0x0F)
        {
        }

    }
    /**************************************************************************/
    // Classes to represent CIL Byte Codes
    /**************************************************************************/
    /// <summary>
    /// The IL instructions for a method
    /// </summary>
    public class CILInstructions {
        private static readonly uint MaxClauses = 10;
        private static readonly uint ExHeaderSize = 4;
        private static readonly uint FatExClauseSize = 24;
        private static readonly uint SmlExClauseSize = 12;
        private static readonly sbyte maxByteVal = 127;
        private static readonly sbyte minByteVal = -128;
        private static readonly byte maxUByteVal = 255;
        private static readonly int smallSize = 64;
        internal static readonly ushort TinyFormat = 0x2;
        internal static readonly ushort FatFormat = 0x03;
        internal static readonly ushort FatFormatHeader = 0x3003;
        internal static readonly ushort MoreSects = 0x8;
        internal static readonly ushort InitLocals = 0x10;
        private static readonly uint FatSize = 12;
        private static readonly uint FatWords = FatSize/4;
        internal static readonly byte FatExceptTable = 0x41;
        internal static readonly byte SmlExceptTable = 0x01;
        internal static readonly uint EHTable = 0x1;
        internal static readonly uint SectFatFormat = 0x40;
        internal static readonly uint SectMoreSects = 0x80;

        private ArrayList exceptions, sourceLines, defaultLines;
        private SourceFile defaultSourceFile;
        private Stack blockStack;
        //private bool codeChecked = false;
        private static readonly int INITSIZE = 5;
        private CILInstruction[] buffer = new CILInstruction[INITSIZE];
        // REPLACE with ArrayList<CILInstruction> for next version of .NET
        private CILInstruction[] saveBuffer;
        private int tide = 0, saveTide = 0;
        private uint offset = 0;
        private ushort headerFlags = 0;
        private short maxStack;
        private uint paddingNeeded = 0;
        private byte exceptHeader = 0;
        private int currI = -1;
        uint localSigIx = 0;
        int numReplace = 0;
        uint codeSize = 0, exceptSize = 0;
        bool tinyFormat, fatExceptionFormat = false, inserting=false;
        MethodDef thisMeth;

        /*-------------------- Constructors ---------------------------------*/

        internal CILInstructions(MethodDef meth)
        {
            thisMeth = meth;
        }

        /*--------------------- public general editing methods ---------------------------*/
        /// <summary>
        /// The source file containing these IL instructions
        /// </summary>
        public SourceFile DefaultSourceFile
        {
            get
            {
                return defaultSourceFile;
            }
            set
            {
                defaultSourceFile = value;
            }
        }

        /// <summary>
        /// The number of instructions currently in the buffer. 
        /// </summary>
        public int NumInstructions()
        {
            if(inserting)
                return tide + saveTide;
            return tide;
        }

        /// <summary>
        /// Get the next instruction in the instruction buffer in sequence.  
        /// An internal index is kept to keep track of which instruction was the last
        /// retrieved by this method.  On the first call, the first instruction in 
        /// the buffer is retrieved.  The instruction index may be zeroed 
        /// using ResetInstCounter().  This method cannot be called when in "insert" mode.
        /// </summary>
        /// <returns></returns>
        public CILInstruction GetNextInstruction()
        {
            if(inserting)
                throw new Exception("Cannot access next instruction during insert");
            if(currI+1 < tide)
                return buffer[++currI];
            return null;
        }

        /// <summary>
        /// Get the previous instruction in the instruction buffer in sequence.  
        /// An internal index is kept to keep track of which instruction was the last
        /// retrieved by this method. This method cannot be called when in "insert" mode.
        /// </summary>
        /// <returns></returns>
        public CILInstruction GetPrevInstruction()
        {
            if(inserting)
                throw new Exception("Cannot access previous instruction during insert");
            if(currI > 0)
                return buffer[--currI];
            return null;
        }

        /// <summary>
        /// Reset the counter for GetNextInstuction to the first instruction.
        /// This method cannot be called when in "insert" mode.
        /// </summary>
        public void ResetInstCounter()
        {
            if(inserting)
                throw new Exception("Cannot reset instruction counter during insert");
            currI = -1;
        }

        /// <summary>
        /// Reset the counter for GetNextInstuction to the first instruction.
        /// This method cannot be called when in "insert" mode.
        /// </summary>
        public void EndInstCounter()
        {
            if(inserting)
                throw new Exception("Cannot reset instruction counter during insert");
            currI = tide;
        }

        /// <summary>
        /// Get all the IL instructions.
        /// This method cannot be called when in "insert" mode.
        /// </summary>
        /// <returns></returns>
        public CILInstruction[] GetInstructions()
        {
            if(inserting)
                throw new Exception("Cannot get instructions during insert");
            return buffer;
        }

        /// <summary>
        /// Set the instruction to be the new array of instructions, this will replace
        /// any existing instructions.  This method cannot be called when in "insert" mode.
        /// </summary>
        /// <param name="insts">The new instructions</param>
        public void SetInstructions(CILInstruction[] insts)
        {
            if(inserting)
                throw new Exception("Cannot replace instructions during insert.");
            buffer = insts;
            tide = buffer.Length;
            for(int i=0; i < tide; i++) {
                if(insts[i] == null)
                    tide = i;
                insts[i].index = (uint)i;
            }
        }

        /// <summary>
        /// This method should only be used to insert instructions into a buffer which 
        /// already contains some instructions.
        /// Start inserting instructions into the instruction buffer ie. set the buffer
        /// to "insert" mode.  The position of the insertion will be directly after 
        /// the "current instruction" as used be GetNextInstruction().  The 
        /// instructions to be inserted are any calls to the instruction specific 
        /// methods - Inst, TypeInst, MethodInst, etc.
        /// This method cannot be called if already in "insert" mode.
        /// </summary>
        public void StartInsert()
        {
            if(inserting)
                throw new Exception("Cannot insert into an instruction buffer already in insert mode");
            inserting = true;
            saveTide = tide;
            saveBuffer = buffer;
            tide = 0;
            buffer = new CILInstruction[INITSIZE];
        }

        /// <summary>
        /// Stop inserting instructions into the buffer.  Any instructions added after 
        /// this call will go at the end of the instruction buffer.  
        /// To be used with StartInsert().
        /// This method cannot be called if not in "insert" mode.
        /// </summary>
        public void EndInsert()
        {
            if(!inserting)
                throw new Exception("Cannot stop inserting if not in insert mode");
            CILInstruction[] newInsts = buffer;
            buffer = saveBuffer;
            int numNew = tide;
            tide = saveTide;
            int insPos = currI+1;
            if(numReplace > 0)
                insPos--;
            InsertInstructions(insPos, newInsts, numNew);
            inserting = false;
            numReplace = 0;
        }

        /// <summary>
        /// Check if the buffer is ready for insertion of extra instructions.
        /// The buffer only needs to be in insert mode when instructions need
        /// to be added to existing instructions, not for addition of instructions
        /// to the end of the buffer.
        /// </summary>
        /// <returns></returns>
        public bool InInsertMode()
        {
            return inserting;
        }

        /// <summary>
        /// Remove the instruction at a specified position from the buffer.  If you 
        /// remove the "current" instruction (from GetNext or GetPrev) then the
        /// "current" instruction becomes the instruction before that in the buffer.
        /// </summary>
        /// <param name="pos">position of the instruction to be removed</param>
        public void RemoveInstruction(int pos)
        {
            if(pos < 0)
                return;
            for(int i=pos; i < tide-1; i++) {
                buffer[i] = buffer[i+1];
                buffer[i].index = (uint)i;
            }
            tide--;
            if(pos == currI)
                currI = pos-1;
        }

        /// <summary>
        /// Remove the instructions from position "startRange" to (and including)
        /// position "endRange" from the buffer.  If the range removed contains the
        /// "current" instruction (from GetNext or GetPrev) then the "current" 
        /// instruction becomes the instruction before startRange in the buffer.
        /// </summary>
        public void RemoveInstructions(int startRange, int endRange)
        {
            if(startRange < 0)
                startRange = 0;
            if(endRange >= tide-1) {// cut to startRange
                tide = startRange;
                return;
            }
            int offset = endRange-startRange+1;
            for(int i=endRange+1; i < tide; i++) {
                buffer[i-offset] = buffer[i];
                buffer[i-offset].index = (uint)(i-offset);
            }
            tide -= offset;
            if((currI >= startRange) && (currI <= endRange))
                currI = startRange-1;
        }

        /// <summary>
        /// Replace a single IL instruction at position pos in the buffer 
        /// with some new instruction(s).  This removes the instruction and puts 
        /// the instruction buffer into "insert" mode at the position of the removed 
        /// instruction.  EndInsert must be called to insert the new instructions.
        /// This method cannot be called when in "insert" mode.
        /// </summary>
        /// <param name="pos">position of the instruction to be replaced</param>
        public void ReplaceInstruction(int pos)
        {
            if(inserting)
                throw new Exception("Cannot replace instructions during insert.");
            if((pos > 0) || (pos < tide)) {
                numReplace = 1;
                StartInsert();
            }
        }

        /// <summary>
        /// Replace a number of IL instructions beginning at position pos in the buffer 
        /// with some new instruction(s).  This removes the instructions and puts 
        /// the instruction buffer into "insert" mode at the position of the removed 
        /// instructions.  EndInsert must be called to insert the new instructions.
        /// The instructions from index "from" up to and including index "to" will
        /// be replaced by the new instructions entered.
        /// This method cannot be called when in "insert" mode.
        /// </summary>
        /// <param name="from">the index to start replacing instruction from</param>
        /// <param name="to">the last index of the instructions to be replaced</param>
        public void ReplaceInstruction(int from, int to)
        {
            if(inserting)
                throw new Exception("Cannot replace instructions during insert.");
            if((from < 0) || (from >= tide) || (to < 0))
                throw new Exception("replace index is out of range");
            if(to >=tide)
                to = tide-1;
            numReplace = to - from + 1;
            StartInsert();
        }

        /*---------------- public instruction specific methods ------------------------*/

        /// <summary>
        /// Add a simple IL instruction
        /// </summary>
        /// <param name="inst">the IL instruction</param>
        public void Inst(Op inst)
        {
            AddToBuffer(new Instr(inst));
        }

        /// <summary>
        /// Add an IL instruction with an integer parameter
        /// </summary>
        /// <param name="inst">the IL instruction</param>
        /// <param name="val">the integer parameter value</param>
        public void IntInst(IntOp inst, int val)
        {
            if((inst == IntOp.ldc_i4_s) || (inst == IntOp.ldc_i4))
                AddToBuffer(new IntInstr(inst, val));
            else
                AddToBuffer(new UIntInstr(inst, (uint)val));
        }

        /// <summary>
        /// Add the load long instruction
        /// </summary>
        /// <param name="cVal">the long value</param>
        public void ldc_i8(long cVal)
        {
            AddToBuffer(new LongInstr(SpecialOp.ldc_i8, cVal));
        }

        /// <summary>
        /// Add the load float32 instruction
        /// </summary>
        /// <param name="cVal">the float value</param>
        public void ldc_r4(float cVal)
        {
            AddToBuffer(new FloatInstr(SpecialOp.ldc_r4, cVal));
        }

        /// <summary>
        /// Add the load float64 instruction
        /// </summary>
        /// <param name="cVal">the float value</param>
        public void ldc_r8(double cVal)
        {
            AddToBuffer(new DoubleInstr(SpecialOp.ldc_r8, cVal));
        }

        /// <summary>
        /// Add the load string instruction
        /// </summary>
        /// <param name="str">the string value</param>
        public void ldstr(string str)
        {
            AddToBuffer(new StringInstr(SpecialOp.ldstr, str));
        }

        /// <summary>
        /// Add the calli instruction
        /// </summary>
        /// <param name="sig">the signature for the calli</param>
        public void calli(CalliSig sig)
        {
            AddToBuffer(new SigInstr(SpecialOp.calli, sig));
        }

        /// <summary>
        /// Create a new CIL label.  To place the label in the CIL instruction
        /// stream use CodeLabel.
        /// </summary>
        /// <returns>a new CIL label</returns>
        public CILLabel NewLabel()
        {
            return new CILLabel();
        }

        /// <summary>
        /// Create a new label at this position in the code buffer
        /// </summary>
        /// <returns>the label at the current position</returns>
        public CILLabel NewCodedLabel()
        {
            CILLabel lab = new CILLabel();
            AddToBuffer(lab);
            return lab;
        }

        /// <summary>
        /// Add a label to the CIL instructions
        /// </summary>
        /// <param name="lab">the label to be added</param>
        public void CodeLabel(CILLabel lab)
        {
            AddToBuffer(lab);
        }

        /// <summary>
        /// Add an instruction with a field parameter
        /// </summary>
        /// <param name="inst">the CIL instruction</param>
        /// <param name="f">the field parameter</param>
        public void FieldInst(FieldOp inst, Field f)
        {
            if(f is FieldDef)
                if(((FieldDef)f).GetScope() != thisMeth.GetScope())
                    throw new DescriptorException();
            AddToBuffer(new FieldInstr(inst, f));
        }

        /// <summary>
        /// Add an instruction with a method parameter
        /// </summary>
        /// <param name="inst">the CIL instruction</param>
        /// <param name="m">the method parameter</param>
        public void MethInst(MethodOp inst, Method m)
        {
            System.Diagnostics.Debug.Assert(m != null);
            if(m is MethodDef)
                if(((MethodDef)m).GetScope() != thisMeth.GetScope())
                    throw new DescriptorException();
            AddToBuffer(new MethInstr(inst, m));
        }

        /// <summary>
        /// Add an instruction with a type parameter
        /// </summary>
        /// <param name="inst">the CIL instruction</param>
        /// <param name="aType">the type argument for the CIL instruction</param>
        public void TypeInst(TypeOp inst, Type aType)
        {
            if(aType is ClassDef) {
                if(((ClassDef)aType).GetScope() != thisMeth.GetScope())
                    throw new DescriptorException();
            }
            AddToBuffer(new TypeInstr(inst, aType));
        }

        /// <summary>
        /// Add a branch instruction
        /// </summary>
        /// <param name="inst">the branch instruction</param>
        /// <param name="lab">the label that is the target of the branch</param>
        public void Branch(BranchOp inst, CILLabel lab)
        {
            AddToBuffer(new BranchInstr(inst, lab));
        }

        /// <summary>
        /// Add a switch instruction
        /// </summary>
        /// <param name="labs">the target labels for the switch</param>
        public void Switch(CILLabel[] labs)
        {
            AddToBuffer(new SwitchInstr(labs));
        }

        /// <summary>
        /// Add a byte to the CIL instructions (.emitbyte)
        /// </summary>
        /// <param name="bVal"></param>
        public void emitbyte(byte bVal)
        {
            AddToBuffer(new CILByte(bVal));
        }

        /// <summary>
        /// Add an instruction which puts an integer on TOS.  This method
        /// selects the correct instruction based on the value of the integer.
        /// </summary>
        /// <param name="i">the integer value</param>
        public void PushInt(int i)
        {
            if(i == -1) {
                AddToBuffer(new Instr(Op.ldc_i4_m1));
            } else if((i >= 0) && (i <= 8)) {
                Op op = (Op)(Op.ldc_i4_0 + i);
                AddToBuffer(new Instr(op));
            } else if((i >= minByteVal) && (i <= maxByteVal)) {
                AddToBuffer(new IntInstr(IntOp.ldc_i4_s, i));
            } else {
                AddToBuffer(new IntInstr(IntOp.ldc_i4, i));
            }
        }

        /// <summary>
        /// Add the instruction to load a long on TOS
        /// </summary>
        /// <param name="l">the long value</param>
        public void PushLong(long l)
        {
            AddToBuffer(new LongInstr(SpecialOp.ldc_i8, l));
        }

        /// <summary>
        /// Add an instruction to push the boolean value true on TOS
        /// </summary>
        public void PushTrue()
        {
            AddToBuffer(new Instr(Op.ldc_i4_1));
        }

        /// <summary>
        ///  Add an instruction to push the boolean value false on TOS
        /// </summary>
        public void PushFalse()
        {
            AddToBuffer(new Instr(Op.ldc_i4_0));
        }

        /// <summary>
        /// Add the instruction to load an argument on TOS.  This method
        /// selects the correct instruction based on the value of argNo
        /// </summary>
        /// <param name="argNo">the number of the argument</param>
        public void LoadArg(int argNo)
        {
            if(argNo < 4) {
                Op op = (Op)Op.ldarg_0 + argNo;
                AddToBuffer(new Instr(op));
            } else if(argNo <= maxUByteVal) {
                AddToBuffer(new UIntInstr(IntOp.ldarg_s, (uint)argNo));
            } else {
                AddToBuffer(new UIntInstr(IntOp.ldarg, (uint)argNo));
            }
        }

        /// <summary>
        /// Add the instruction to load the address of an argument on TOS.
        /// This method selects the correct instruction based on the value
        /// of argNo.
        /// </summary>
        /// <param name="argNo">the number of the argument</param>
        public void LoadArgAdr(int argNo)
        {
            if(argNo <= maxUByteVal) {
                AddToBuffer(new UIntInstr(IntOp.ldarga_s, (uint)argNo));
            } else {
                AddToBuffer(new UIntInstr(IntOp.ldarga, (uint)argNo));
            }
        }

        /// <summary>
        /// Add the instruction to load a local on TOS.  This method selects
        /// the correct instruction based on the value of locNo.
        /// </summary>
        /// <param name="locNo">the number of the local to load</param>
        public void LoadLocal(int locNo)
        {
            if(locNo < 4) {
                Op op = (Op)Op.ldloc_0 + locNo;
                AddToBuffer(new Instr(op));
            } else if(locNo <= maxUByteVal) {
                AddToBuffer(new UIntInstr(IntOp.ldloc_s, (uint)locNo));
            } else {
                AddToBuffer(new UIntInstr(IntOp.ldloc, (uint)locNo));
            }
        }

        /// <summary>
        /// Add the instruction to load the address of a local on TOS.
        /// This method selects the correct instruction based on the 
        /// value of locNo.
        /// </summary>
        /// <param name="locNo">the number of the local</param>
        public void LoadLocalAdr(int locNo)
        {
            if(locNo <= maxUByteVal) {
                AddToBuffer(new UIntInstr(IntOp.ldloca_s, (uint)locNo));
            } else {
                AddToBuffer(new UIntInstr(IntOp.ldloca, (uint)locNo));
            }
        }

        /// <summary>
        /// Add the instruction to store to an argument.  This method
        /// selects the correct instruction based on the value of argNo.
        /// </summary>
        /// <param name="argNo">the argument to be stored to</param>
        public void StoreArg(int argNo)
        {
            if(argNo <= maxUByteVal) {
                AddToBuffer(new UIntInstr(IntOp.starg_s, (uint)argNo));
            } else {
                AddToBuffer(new UIntInstr(IntOp.starg, (uint)argNo));
            }
        }

        /// <summary>
        /// Add the instruction to store to a local.  This method selects
        /// the correct instruction based on the value of locNo.
        /// </summary>
        /// <param name="locNo">the local to be stored to</param>
        public void StoreLocal(int locNo)
        {
            if(locNo < 4) {
                Op op = (Op)Op.stloc_0 + locNo;
                AddToBuffer(new Instr(op));
            } else if(locNo <= maxUByteVal) {
                AddToBuffer(new UIntInstr(IntOp.stloc_s, (uint)locNo));
            } else {
                AddToBuffer(new UIntInstr(IntOp.stloc, (uint)locNo));
            }
        }

        public void Line(uint num, uint startCol)
        {
            AddToBuffer(new Line(num, startCol));
        }

        public void Line(uint num, uint startCol, uint endCol)
        {
            AddToBuffer(new Line(num, startCol, num, endCol));
        }

        public void Line(uint startNum, uint startCol, uint endNum, uint endCol)
        {
            AddToBuffer(new Line(startNum, startCol, endNum, endCol));
        }

        public void Line(uint startNum, uint startCol, uint endNum, uint endCol, SourceFile sFile)
        {
            AddToBuffer(new Line(startNum, startCol, endNum, endCol, sFile));
        }
        /*
         *  public void RenameLocal(Local loc, string newName) {
         *    AddToBuffer(new LocalScope(loc,newName));
         *  }
         *
         *  public void ReleaseLocal(Local loc) {
         *    AddToBuffer(new LocalScope(loc));
         *  }
         */

        /*   // Added to enable PDB generation
         *  public Scope CurrentScope {
         *    get { return currentScope; }
         *  }
         *
         *  // Added to enable PDB generation
         *  public void OpenScope() {
         *    currentScope = new Scope(currentScope, thisMeth);
         *    AddToBuffer(new OpenScope(currentScope));
         *  }
         *
         *  // Added to enable PDB generation
         *  public void CloseScope() {
         *    AddToBuffer(new CloseScope(currentScope));
         *    currentScope = currentScope._parent;
         *  }
         *
         *  // Added to enable PDB generation
         *  public LocalBinding BindLocal(string name, int index) {
         *    return currentScope.AddLocalBinding(name, index);
         *  }
         *
         *  // Added to enable PDB generation
         *  public LocalVisBinding StartLocalBinding(string name, int index) {
         *    LocalVisBinding binding = currentScope.AddLocalVisBinding(name, index);
         *    StartVisBinding inst = new StartVisBinding(binding);
         *    binding._start = inst;
         *    AddToBuffer(inst);
         *    return binding;
         *  }
         *
         *  // Added to enable PDB generation
         *  public void EndLocalBinding(LocalVisBinding binding) {
         *    EndVisBinding inst = new EndVisBinding(binding);
         *    binding._end = inst;
         *    AddToBuffer(inst);
         *  }
         */
        /// <summary>
        /// Mark this position as the start of a new block
        /// (try, catch, filter, finally or fault)
        /// </summary>
        public void StartBlock()
        {
            if(blockStack == null)
                blockStack = new Stack();
            blockStack.Push(NewCodedLabel());
        }

        /// <summary>
        /// Mark this position as the end of the last started block and
        /// make it a try block.  This try block is added to the current 
        /// instructions (ie do not need to call AddTryBlock)
        /// </summary>
        /// <returns>The try block just ended</returns>
        public TryBlock EndTryBlock()
        {
            TryBlock tBlock = new TryBlock((CILLabel)blockStack.Pop(), NewCodedLabel());
            AddTryBlock(tBlock);
            return tBlock;
        }

        /// <summary>
        /// Mark this position as the end of the last started block and
        /// make it a catch block.  This catch block is associated with the
        /// specified try block.
        /// </summary>
        /// <param name="exceptType">the exception type to be caught</param>
        /// <param name="tryBlock">the try block associated with this catch block</param>
        public void EndCatchBlock(Class exceptType, TryBlock tryBlock)
        {
            Catch catchBlock = new Catch(exceptType, (CILLabel)blockStack.Pop(), NewCodedLabel());
            tryBlock.AddHandler(catchBlock);
        }

        /// <summary>
        /// Mark this position as the end of the last started block and
        /// make it a filter block.  This filter block is associated with the
        /// specified try block.  The format is:
        /// filterLab:   ...
        ///              ...
        /// filterHandler :  ...
        ///                  ...             
        /// </summary>
        /// <param name="filterLab">the label where the filter code is</param>
        /// <param name="tryBlock">the try block associated with this filter block</param>
        public void EndFilterBlock(CILLabel filterLab, TryBlock tryBlock)
        {
            Filter filBlock = new Filter(filterLab, (CILLabel)blockStack.Pop(), NewCodedLabel());
            tryBlock.AddHandler(filBlock);
        }

        /// <summary>
        /// Mark this position as the end of the last started block and
        /// make it a finally block.  This finally block is associated with the
        /// specified try block.
        /// </summary>
        /// <param name="tryBlock">the try block associated with this finally block</param>
        public void EndFinallyBlock(TryBlock tryBlock)
        {
            Finally finBlock= new Finally((CILLabel)blockStack.Pop(), NewCodedLabel());
            tryBlock.AddHandler(finBlock);
        }

        /// <summary>
        /// Mark this position as the end of the last started block and
        /// make it a fault block.  This fault block is associated with the
        /// specified try block.
        /// </summary>
        /// <param name="tryBlock">the try block associated with this fault block</param>
        public void EndFaultBlock(TryBlock tryBlock)
        {
            Fault fBlock= new Fault((CILLabel)blockStack.Pop(), NewCodedLabel());
            tryBlock.AddHandler(fBlock);
        }

        public void AddTryBlock(TryBlock tryBlock)
        {
            if(exceptions == null)
                exceptions = new ArrayList();
            else if(exceptions.Contains(tryBlock))
                return;
            exceptions.Add(tryBlock);
        }

        /*------------------------- private methods ----------------------------*/

        private void AddToBuffer(CILInstruction inst)
        {
            if(tide >= buffer.Length) {
                CILInstruction[] tmp = buffer;
                buffer = new CILInstruction[tmp.Length * 2];
                for(int i=0; i < tide; i++) {
                    buffer[i] = tmp[i];
                }
            }
            //Console.WriteLine("Adding instruction at offset " + offset + " with size " + inst.size);
            //inst.offset = offset;
            //offset += inst.size;
            inst.index = (uint)tide;
            buffer[tide++] = inst;
        }

        private void UpdateIndexesFrom(int ix)
        {
            for(int i=ix; i < tide; i++) {
                buffer[i].index = (uint)i;
            }
        }

        private void InsertInstructions(int ix, CILInstruction[] newInsts, int numNew)
        {
            CILInstruction[] newBuff = buffer, oldBuff = buffer;
            int newSize = tide + numNew - numReplace;
            if(buffer.Length < newSize) {
                newBuff = new CILInstruction[newSize];
                for(int i=0; i < ix; i++) {
                    newBuff[i] = oldBuff[i];
                }
            }
            // shuffle up
            int offset = numNew-numReplace;
            int end = ix + numReplace;
            for(int i=tide-1; i >= end; i--) {
                newBuff[i+offset] = oldBuff[i];
            }
            // insert new instructions
            for(int i=0; i < numNew; i++) {
                newBuff[ix+i] = newInsts[i];
            }
            buffer = newBuff;
            tide += numNew - numReplace;
            UpdateIndexesFrom(ix);
        }

        internal static CILLabel GetLabel(ArrayList labs, uint targetOffset)
        {
            CILLabel lab;
            int i=0;
            while((i < labs.Count) && (((CILLabel)labs[i]).offset < targetOffset))
                i++;
            if(i < labs.Count) {
                if(((CILLabel)labs[i]).offset == targetOffset) // existing label
                    lab = (CILLabel)labs[i];
                else {
                    lab = new CILLabel(targetOffset);
                    labs.Insert(i, lab);
                }
            } else {
                lab = new CILLabel(targetOffset);
                labs.Add(lab);
            }
            return lab;
        }

        internal void AddEHClause(EHClause ehc)
        {
            if(exceptions == null)
                exceptions = new ArrayList();
            exceptions.Add(ehc);
        }

        internal void SetAndResolveInstructions(CILInstruction[] insts)
        {
            offset = 0;
            ArrayList labels = new ArrayList();
            for(int i=0; i < insts.Length; i++) {
                insts[i].offset = offset;
                offset += insts[i].size;
                if(insts[i] is BranchInstr) {
                    ((BranchInstr)insts[i]).MakeTargetLabel(labels);
                } else if(insts[i] is SwitchInstr) {
                    ((SwitchInstr)insts[i]).MakeTargetLabels(labels);
                }
            }
            if(exceptions != null) {
                for(int i=0; i < exceptions.Count; i++) {
                    exceptions[i] = ((EHClause)exceptions[i]).MakeTryBlock(labels);
                }
            }
            if(labels.Count == 0) {
                buffer = insts;
                tide = buffer.Length;
                return;
            }
            buffer = new CILInstruction[insts.Length + labels.Count];
            int currentPos = 0;
            tide = 0;
            for(int i=0; i < labels.Count; i++) {
                CILLabel lab = (CILLabel)labels[i];
                while((currentPos < insts.Length) && (insts[currentPos].offset < lab.offset))
                    buffer[tide++] = insts[currentPos++];
                buffer[tide++] = lab;
            }
            while(currentPos < insts.Length) {
                buffer[tide++] = insts[currentPos++];
            }
        }

        internal uint GetCodeSize()
        {
            return codeSize + paddingNeeded + exceptSize;
        }

        internal void BuildTables(MetaDataOut md)
        {
            for(int i=0; i < tide; i++) {
                buffer[i].BuildTables(md);
            }
            if(exceptions != null) {
                for(int i=0; i < exceptions.Count; i++) {
                    ((TryBlock)exceptions[i]).BuildTables(md);
                }
            }
        }

        internal void ChangeRefsToDefs(ClassDef newType, ClassDef[] oldTypes)
        {
            for(int i=0; i < tide; i++) {
                if(buffer[i] is SigInstr) {
                    CalliSig sig = ((SigInstr)buffer[i]).GetSig();
                    sig.ChangeRefsToDefs(newType, oldTypes);
                } else if(buffer[i] is TypeInstr) {
                    TypeInstr tinst = (TypeInstr)buffer[i];
                    if(tinst.GetTypeArg() is ClassDef) {
                        ClassDef iType = (ClassDef)tinst.GetTypeArg();
                        bool changed = false;
                        for(int j=0; (j < oldTypes.Length) && !changed; j++) {
                            if(iType == oldTypes[j])
                                tinst.SetTypeArg(newType);
                        }
                    }
                }
            }
        }

        internal void AddToLines(Line line)
        {
            if((line.sourceFile == null) || (line.sourceFile.Match(defaultSourceFile))) {
                if(defaultLines == null) {
                    if(defaultSourceFile == null)
                        throw new Exception("No Default Source File Set");
                    defaultLines = new ArrayList();
                }
                defaultLines.Add(line);
                return;
            }
            if(sourceLines == null) {
                sourceLines = new ArrayList();
            } else {
                for(int i=0; i < sourceLines.Count; i++) {
                    ArrayList lineList = (ArrayList)sourceLines[i];
                    if(((Line)lineList[0]).sourceFile.Match(line.sourceFile)) {
                        lineList.Add(line);
                        return;
                    }
                }
                ArrayList newList = new ArrayList();
                newList.Add(line);
                sourceLines.Add(newList);
            }
        }

        internal void CheckCode(uint locSigIx, bool initLocals, int maxStack, MetaDataOut metaData)
        {
            if(tide == 0)
                return;
            offset = 0;
            for(int i=0; i < tide; i++) {
                buffer[i].offset = offset;
                offset += buffer[i].size;
                if(buffer[i] is Line)
                    AddToLines((Line)buffer[i]);
            }
            bool changed = true;
            while(changed) {
                changed = false;
                Line prevLine = null;
                for(int i=0; i < tide; i++) {
                    if(buffer[i] is Line) {
                        if(prevLine != null)
                            prevLine.CalcEnd((Line)buffer[i]);
                        prevLine = (Line)buffer[i];
                    }
                    changed = buffer[i].Check(metaData) || changed;
                }
                if(prevLine != null)
                    prevLine.Last();
                if(changed) {
                    for(int i=1; i < tide; i++) {
                        buffer[i].offset = buffer[i-1].offset + buffer[i-1].size;
                    }
                    offset = buffer[tide-1].offset + buffer[tide-1].size;
                }
            }
            codeSize = offset;
            if(Diag.DiagOn)
                Console.WriteLine("codeSize before header added = " + codeSize);
            if(maxStack == 0)
                this.maxStack = 8;
            else
                this.maxStack = (short)maxStack;
            if((offset < smallSize) && (maxStack <= 8) && (locSigIx == 0) && (exceptions == null)) {
                // can use tiny header
                if(Diag.DiagOn)
                    Console.WriteLine("Tiny Header");
                tinyFormat = true;
                headerFlags = (ushort)(TinyFormat | ((ushort)codeSize << 2));
                codeSize++;
                if((codeSize % 4) != 0) {
                    paddingNeeded = 4 - (codeSize % 4);
                }
            } else {
                if(Diag.DiagOn)
                    Console.WriteLine("Fat Header");
                tinyFormat = false;
                localSigIx = locSigIx;
                //this.maxStack = (short)maxStack;
                headerFlags = FatFormatHeader;
                if(exceptions != null) {
                    // Console.WriteLine("Got exceptions");
                    headerFlags |= MoreSects;
                    uint numExceptClauses = 0;
                    for(int i=0; i < exceptions.Count; i++) {
                        TryBlock tryBlock = (TryBlock)exceptions[i];
                        tryBlock.SetSize();
                        numExceptClauses += (uint)tryBlock.NumHandlers();
                        if(tryBlock.isFat())
                            fatExceptionFormat = true;
                    }
                    if(numExceptClauses > MaxClauses)
                        fatExceptionFormat = true;
                    if(Diag.DiagOn)
                        Console.WriteLine("numexceptclauses = " + numExceptClauses);
                    if(fatExceptionFormat) {
                        if(Diag.DiagOn)
                            Console.WriteLine("Fat exception format");
                        exceptHeader = FatExceptTable;
                        exceptSize = ExHeaderSize + numExceptClauses * FatExClauseSize;
                    } else {
                        if(Diag.DiagOn)
                            Console.WriteLine("Tiny exception format");
                        exceptHeader = SmlExceptTable;
                        exceptSize = ExHeaderSize + numExceptClauses * SmlExClauseSize;
                    }
                    if(Diag.DiagOn)
                        Console.WriteLine("exceptSize = " + exceptSize);
                }
                if(initLocals)
                    headerFlags |= InitLocals;
                if((offset % 4) != 0) {
                    paddingNeeded = 4 - (offset % 4);
                }
                codeSize += FatSize;
            }
            if(Diag.DiagOn)
                Console.WriteLine("codeSize = " + codeSize + "  headerFlags = " + Hex.Short(headerFlags));
        }

        internal void Write(PEWriter output)
        {
            if(Diag.DiagOn)
                Console.WriteLine("Writing header flags = " + Hex.Short(headerFlags));
            if(tinyFormat) {
                if(Diag.DiagOn)
                    Console.WriteLine("Writing tiny code");
                output.Write((byte)headerFlags);
            } else {
                if(Diag.DiagOn)
                    Console.WriteLine("Writing fat code");
                output.Write(headerFlags);
                output.Write((ushort)maxStack);
                output.Write(offset);
                output.Write(localSigIx);
            }
            if(Diag.DiagOn) {
                Console.WriteLine(Hex.Int(tide) + " CIL instructions");
                Console.WriteLine("starting instructions at " + output.Seek(0, SeekOrigin.Current));
            }
            /*
             *    // Added to enable PDB generation
             *    if (output.pdbWriter != null) {
             *	    output.pdbWriter.OpenMethod((int) thisMeth.Token(), thisMeth);
             *     }
             */
            for(int i=0; i < tide; i++) {
                buffer[i].Write(output);
            }
            /*
             *    // Added to enable PDB generation
             *    if (output.pdbWriter != null && tide > 0) {
             *      output.pdbWriter.CloseMethod();
             *    }
             */
            if(Diag.DiagOn)
                Console.WriteLine("ending instructions at " + output.Seek(0, SeekOrigin.Current));
            for(int i=0; i < paddingNeeded; i++) {
                output.Write((byte)0);
            }
            if(exceptions != null) {
                // Console.WriteLine("Writing exceptions");
                // Console.WriteLine("header = " + Hex.Short(exceptHeader) + " exceptSize = " + Hex.Int(exceptSize));
                output.Write(exceptHeader);
                output.Write3Bytes((uint)exceptSize);
                for(int i=0; i < exceptions.Count; i++) {
                    TryBlock tryBlock = (TryBlock)exceptions[i];
                    tryBlock.Write(output, fatExceptionFormat);
                }
            }
        }

    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for an IL instruction
    /// </summary>
	public abstract partial class CILInstruction {
        protected static readonly sbyte maxByteVal = 127;
        protected static readonly sbyte minByteVal = -128;
        protected static readonly byte leadByte = 0xFE;
        protected static readonly uint USHeapIndex = 0x70000000;
        protected static readonly uint longInstrStart = (uint)Op.arglist;
        internal bool twoByteInstr = false;
        internal uint size = 1;
        internal uint offset, index;

        internal virtual bool Check(MetaDataOut md)
        {
            return false;
        }

        internal virtual void Resolve()
        {
        }

        public int GetPos()
        {
            return (int)index;
        }

        internal abstract string GetInstName();

        internal virtual void BuildTables(MetaDataOut md)
        {
        }

        internal virtual void Write(PEWriter output)
        {
        }

    }

    /**************************************************************************/
    public class CILByte : CILInstruction {
        byte byteVal;

        /*-------------------- Constructors ---------------------------------*/

        internal CILByte(byte bVal)
        {
            byteVal = bVal;
        }

        public byte GetByte()
        {
            return byteVal;
        }

        internal override string GetInstName()
        {
            return Hex.Byte(byteVal);
        }

        internal override void Write(PEWriter output)
        {
            output.Write(byteVal);
        }

    }

    /**************************************************************************/
    public partial class Instr : CILInstruction {
        protected uint instr;

        /*-------------------- Constructors ---------------------------------*/

        public Instr(Op inst)
        {
            instr = (uint)inst;
            if(instr >= longInstrStart) {
                instr -= longInstrStart;
                twoByteInstr = true;
                size++;
            }
        }

        internal Instr(uint inst)
        {
            instr = (uint)inst;
            if(instr >= longInstrStart) {
                instr -= longInstrStart;
                twoByteInstr = true;
                size++;
            }
        }

        public Op GetOp()
        {
            if(twoByteInstr)
                return (Op)(longInstrStart + instr);
            return (Op)instr;
        }

        internal override string GetInstName()
        {
            Op opInst = GetOp();
            return "" + opInst;
        }

        internal override void Write(PEWriter output)
        {
            //Console.WriteLine("Writing instruction " + instr + " with size " + size);
            if(twoByteInstr)
                output.Write(leadByte);
            output.Write((byte)instr);
        }

    }

    /**************************************************************************/
    public class IntInstr : Instr {
        int val;
        bool byteNum;

        /*-------------------- Constructors ---------------------------------*/

        public IntInstr(IntOp inst, int num)
            : base((uint)inst)
        {
            byteNum = inst == IntOp.ldc_i4_s;
            val = num;
            if(byteNum)
                size++;
            else
                size += 4;
        }

        public int GetInt()
        {
            return val;
        }
        public void SetInt(int num)
        {
            val = num;
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            if(byteNum)
                output.Write((sbyte)val);
            else
                output.Write(val);
        }

    }

    /**************************************************************************/
    public class UIntInstr : Instr {
        uint val;
        bool byteNum;

        /*-------------------- Constructors ---------------------------------*/

        public UIntInstr(IntOp inst, uint num)
            : base((uint)inst)
        {
            byteNum = (inst < IntOp.ldc_i4_s) || (inst == IntOp.unaligned);
            val = num;
            if(byteNum)
                size++;
            else
                size += 2;
        }

        public uint GetUInt()
        {
            return val;
        }
        public void SetUInt(uint num)
        {
            val = num;
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            if(byteNum)
                output.Write((byte)val);
            else
                output.Write((ushort)val);
        }

    }

    /**************************************************************************/
    public class LongInstr : Instr {
        long val;

        /*-------------------- Constructors ---------------------------------*/

        public LongInstr(SpecialOp inst, long l)
            : base((uint)inst)
        {
            val = l;
            size += 8;
        }

        public long GetLong()
        {
            return val;
        }
        public void SetLong(long num)
        {
            val = num;
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(val);
        }

    }

    /**************************************************************************/
    public class FloatInstr : Instr {
        float fVal;

        /*-------------------- Constructors ---------------------------------*/

        public FloatInstr(SpecialOp inst, float f)
            : base((uint)inst)
        {
            fVal = f;
            size += 4;
        }

        public float GetFloat()
        {
            return fVal;
        }
        public void SetFloat(float num)
        {
            fVal = num;
        }

        internal sealed override void Write(PEWriter output)
        {
            output.Write((byte)0x22);
            output.Write(fVal);
        }

    }

    /**************************************************************************/
    public class DoubleInstr : Instr {
        double val;

        /*-------------------- Constructors ---------------------------------*/

        public DoubleInstr(SpecialOp inst, double d)
            : base((uint)inst)
        {
            val = d;
            size += 8;
        }

        public double GetDouble()
        {
            return val;
        }
        public void SetDouble(double num)
        {
            val = num;
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(val);
        }

    }

    /**************************************************************************/
    public class StringInstr : Instr {
        string val;
        uint strIndex;

        /*-------------------- Constructors ---------------------------------*/

        public StringInstr(SpecialOp inst, string str)
            : base((uint)inst)
        {
            val = str;
            size += 4;
        }

        public string GetString()
        {
            return val;
        }
        public void SetString(string str)
        {
            val = str;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(Diag.DiagOn)
                Console.WriteLine("Adding a code string to the US heap");
            strIndex = md.AddToUSHeap(val);
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(USHeapIndex  | strIndex);
        }

    }

    /**************************************************************************/
    public class CILLabel : CILInstruction {

        /*-------------------- Constructors ---------------------------------*/

        public CILLabel()
        {
            size = 0;
        }

        internal CILLabel(uint offs)
        {
            size = 0;
            offset = offs;
        }

        internal uint GetLabelOffset()
        {
            return offset;
        }

        internal override string GetInstName()
        {
            return "Label";
        }


    }

    /**************************************************************************/
    public abstract class DebugInst : CILInstruction {
    }

    /**************************************************************************/
    public class Line : DebugInst {
        private static uint MaxCol = 100;
        uint startLine, startCol, endLine, endCol;
        bool hasEnd = false;
        internal SourceFile sourceFile;

        /*-------------------- Constructors ---------------------------------*/

        internal Line(uint sLine, uint sCol)
        {
            startLine = sLine;
            startCol = sCol;
            size = 0;
        }

        internal Line(uint sLine, uint sCol, uint eLine, uint eCol)
        {
            startLine = sLine;
            startCol = sCol;
            endLine = eLine;
            endCol = eCol;
            hasEnd = true;
            size = 0;
        }

        internal Line(uint sLine, uint sCol, uint eLine, uint eCol, SourceFile sFile)
        {
            startLine = sLine;
            startCol = sCol;
            endLine = eLine;
            endCol = eCol;
            hasEnd = true;
            sourceFile = sFile;
            size = 0;
        }

        internal void CalcEnd(Line next)
        {
            if(hasEnd)
                return;
            if(sourceFile != next.sourceFile) {
                endLine = startLine;
                endCol = MaxCol;
            } else {
                endLine = next.startLine;
                endCol = next.endCol - 1;
                if(endCol < 0)
                    endCol = MaxCol;
            }
            hasEnd = true;
        }

        internal void Last()
        {
            if(hasEnd)
                return;
            endLine = startLine;
            endCol = MaxCol;
            hasEnd = true;
        }

        internal override string GetInstName()
        {
            return ".line";
        }
        /*
         *  internal override void Write(PEWriter output) {
         *    if (sourceFile != null) {
         *      output.pdbWriter.AddSequencePoint(sourceFile, (int)offset,
         *         (int)startLine, (int)startCol, (int)endLine, (int)endCol);
         *     }
         *     base.Write (output);
         *   }
         */

    }

    /**************************************************************************  /  
  /*  public class LocalBinding : DebugInst {
   *  internal int _index;
   *  internal string _name;
  //    internal byte[] _sig = null;
   *  internal DebugLocalSig _debugsig;
   *
      /*-------------------- Constructors ---------------------------------*  /
   *
   *  internal LocalBinding(int index, string name) 
   *  {
   *    _index = index;
   *    _name = name;
   *  }
   *
   *  public int Index {
   *    get { return _index; }
   *  }
   *
   *  public string Name {
   *    get { return _name; }
   *  }
   *
   *  internal override string GetInstName() {
   *    return "debug - local binding";
   *  }
   *
   * }
    /*************************************************************************  / 
   * 
   * public class LocalVisBinding : LocalBinding {
   *   internal StartVisBinding _start;
   *   internal EndVisBinding _end;
   *
   *    internal LocalVisBinding(int index, string name) : base(index, name) {
   *    }
   *
   *  }
    /*************************************************************************  / 
   *
   *public class Scope {
   *  private ArrayList _bindings = new ArrayList();
   *  private ArrayList _visBindings = new ArrayList();
   *  internal Scope _parent;
   *  internal MethodDef _thisMeth;

   *  internal Scope(MethodDef thisMeth) : this(null, thisMeth) {
   *  }

   *  internal Scope(Scope parent, MethodDef thisMeth) {
   *    _thisMeth = thisMeth;
   *    _parent = parent;
   *  }

   *  internal LocalBinding AddLocalBinding(string name, int index) {
   *    LocalBinding binding;
   *    if ((binding = FindBinding(name)) != null)
   *      return binding;

   *    binding = new LocalBinding(index, name);
   *    _bindings.Add(binding);
   *    return binding;
   *  }

   *  internal LocalBinding FindBinding(string name) {
   *    foreach (LocalBinding binding in _bindings)
   *      if (binding._name == name)
   *        return binding;
   *    return null;
   *  }

   *  internal LocalBinding FindBinding(int index) {
   *    foreach (LocalBinding binding in _bindings)
   *      if (binding._index == index)
   *        return binding;
   *    return null;
   *  }

   *  internal LocalVisBinding AddLocalVisBinding(string name, int index) {
   *    LocalVisBinding binding = new LocalVisBinding(index, name);
   *    _visBindings.Add(binding);
   *    return binding;
   *  }

   *  public LocalBinding[] LocalBindings {
   *    get { return (LocalBinding[]) _bindings.ToArray(typeof(LocalBinding)); }
   *  }

   *  public LocalVisBinding[] LocalVisBindings {
   *    get { return (LocalVisBinding[]) _visBindings.ToArray(typeof(LocalVisBinding)); }
   *  }

   *  internal void BuildSignatures(MetaDataOut md) {	
   *    if (!md.Debug) return;
   *    if (md.output.pdbWriter.GetVarSigType() != PDB.LocalVarSigType.Token) return; // no need to build signatures now
   *    try {
   *      Local[] locals = _thisMeth.GetLocals();
   *      foreach (LocalBinding binding in _bindings) {
   *        if (binding._debugsig == null) {
   *           locals[binding._index].BuildTables(md);
   *           binding._debugsig = md.GetDebugSig(locals[binding._index]);
   *        }
   *        binding._debugsig.BuildMDTables(md);
   *      }
   *      foreach (LocalVisBinding binding in _visBindings) {
   *        if (binding._start != null && binding._end != null) {
   *          if (binding._debugsig == null) {
   *            binding._debugsig = md.GetDebugSig(locals[binding._index]);
   *          }
   *          binding._debugsig.BuildMDTables(md);
   *        }
   *      }
   *    } catch (Exception e) {
   *      throw new Exception("Exception while writing debug info for: " +
   *                           this._thisMeth.NameString()+"\r\n"+e.ToString());
   *    }
   *  }
   *
   *  internal void WriteLocals(PERWAPI.PDB.PDBWriter writer) {
   *    try {
   *      Local[] locals = _thisMeth.GetLocals();
   *      foreach (LocalBinding binding in _bindings) {
   *        if (writer.GetVarSigType() == PDB.LocalVarSigType.Token) {
   *          if (binding._debugsig != null) {
   *            writer.BindLocal(binding._name, binding._index, binding._debugsig.Token(), 0, 0);
   *          }
   *        } else
   *          writer.BindVisLocal(binding._name, binding._index, locals[binding._index].GetSig(), 0, 0);
   *      }
   *      foreach (LocalVisBinding binding in _visBindings) {
   *        if (binding._start != null && binding._end != null) {
   *          if (writer.GetVarSigType() == PDB.LocalVarSigType.Token) {
   *            if (binding._debugsig != null)
   *              writer.BindLocal(binding._name, binding._index, binding._debugsig.Token(), (int) binding._start.offset, (int) binding._end.offset);
   *          } else 
   *            writer.BindVisLocal(binding._name, binding._index, locals[binding._index].GetSig(), (int) binding._start.offset, (int) binding._end.offset);
   *        }
   *      }
   *    } catch (Exception e) {
   *      throw new Exception("Exception while writing debug info for: " +
   *                           this._thisMeth.NameString()+"\r\n"+e.ToString());
   *    }    
   *  }
   *}
   *
    /*************************************************************************  / 
   *
   *public class OpenScope : DebugInst {
   *  internal Scope _scope;
   *
   *  public OpenScope(Scope scope) {
   *    size = 0;
   *    _scope = scope;
   *  }
   *
   *  internal override string GetInstName() {
   *    return "debug - open scope";
   *  }
   *
   *  internal void BuildSignatures(MetaDataOut md) {
   *    _scope.BuildSignatures(md);
   *  }
   *
   *  internal override void Write(PEWriter output) {
   *    if (output.pdbWriter != null) {
   *      output.pdbWriter.OpenScope((int) offset);
   *      _scope.WriteLocals(output.pdbWriter);
   *    }
   *  }
   *
   *}
    /************************************************************************  /  
   *
   *public class CloseScope : DebugInst {
   *  internal Scope _scope;
   *
   *  public CloseScope(Scope scope) {
   *    size = 0;
   *    _scope = scope;
   *  }
   *
   *  internal override string GetInstName() {
   *    return "debug - close scope";
   *  }
   *
   *  internal override void Write(PEWriter output) {
   *    if (output.pdbWriter != null)
   *      output.pdbWriter.CloseScope((int) offset);
   *  }
   *
   *}  
    /************************************************************************  /  
   *
   *public class StartVisBinding : DebugInst {
   *  internal LocalVisBinding _binding;
   *
   *  public StartVisBinding(LocalVisBinding binding) {
   *    size = 0;
   *    _binding = binding;
   *  }
   *
   *  internal override string GetInstName() {
   *    return "debug - startVis binding";
   *  }
   *
   *}
    /***********************************************************************  /  
   *
   *public class EndVisBinding : DebugInst {
   *  internal LocalVisBinding _binding;
   *
   *  public EndVisBinding(LocalVisBinding binding) {
   *    size = 0;
   *    _binding = binding;
   *}
   *
   *  internal override string GetInstName() {
   *    return "debug - endVis binding";
   *  }
   *
   *}
   */

    /**************************************************************************/

    public class FieldInstr : Instr {
        Field field;

        /*-------------------- Constructors ---------------------------------*/

        public FieldInstr(FieldOp inst, Field f)
            : base((uint)inst)
        {
            field = f;
            size += 4;
        }

        public Field GetField()
        {
            return field;
        }

        public void SetField(Field fld)
        {
            field = fld;
        }

        internal override string GetInstName()
        {
            return "" + (FieldOp)instr;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(field == null)
                throw new InstructionException(IType.fieldOp, instr);
            if(field is FieldRef)
                field.BuildMDTables(md);
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(field.Token());
        }

    }

    /**************************************************************************/
    public class MethInstr : Instr {
        Method meth;

        /*-------------------- Constructors ---------------------------------*/

        public MethInstr(MethodOp inst, Method m)
            : base((uint)inst)
        {
            meth = m;
            size += 4;
        }

        public Method GetMethod()
        {
            return meth;
        }

        public void SetMethod(Method mth)
        {
            meth = mth;
        }

        internal override string GetInstName()
        {
            return "" + (MethodOp)instr;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(meth == null)
                throw new InstructionException(IType.methOp, instr);
            if((meth is MethodRef) || (meth is MethodSpec))
                meth.BuildMDTables(md);
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(meth.Token());
        }

    }

    /**************************************************************************/
    public class SigInstr : Instr {
        CalliSig signature;

        /*-------------------- Constructors ---------------------------------*/

        public SigInstr(SpecialOp inst, CalliSig sig)
            : base((uint)inst)
        {
            signature = sig;
            size += 4;
        }

        public CalliSig GetSig()
        {
            return signature;
        }

        public void SetSig(CalliSig sig)
        {
            signature = sig;
        }

        internal override string GetInstName()
        {
            return "" + (SpecialOp)instr;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(signature == null)
                throw new InstructionException(IType.specialOp, instr);
            signature.BuildMDTables(md);
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(signature.Token());
        }
    }

    /**************************************************************************/
    public class TypeInstr : Instr {
        Type theType;

        /*-------------------- Constructors ---------------------------------*/

        public TypeInstr(TypeOp inst, Type aType)
            : base((uint)inst)
        {
            theType = aType;
            size += 4;
        }

        public Type GetTypeArg()
        {
            return theType;
        }

        public void SetTypeArg(Type ty)
        {
            theType = ty;
        }

        internal override string GetInstName()
        {
            return "" + (TypeOp)instr;
        }

        internal sealed override void BuildTables(MetaDataOut md)
        {
            if(theType == null)
                throw new InstructionException(IType.typeOp, instr);
            theType = theType.AddTypeSpec(md);
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(theType.Token());
        }

    }

    /**************************************************************************/
    public class BranchInstr : Instr {
        CILLabel dest;
        private bool shortVer = true;
        private static readonly byte longInstrOffset = 13;
        private int target = 0;

        /*-------------------- Constructors ---------------------------------*/

        public BranchInstr(BranchOp inst, CILLabel dst)
            : base((uint)inst)
        {
            dest = dst;
            shortVer = (inst < BranchOp.br) || (inst == BranchOp.leave_s);
            if(shortVer)
                size++;
            else
                size += 4;
        }

        internal BranchInstr(uint inst, int dst)
            : base(inst)
        {
            target = dst;
            shortVer = (inst < (uint)BranchOp.br) || (inst == (uint)BranchOp.leave_s);
            if(shortVer)
                size++;
            else
                size += 4;
        }

        public CILLabel GetDest()
        {
            return dest;
        }

        public void SetDest(CILLabel lab)
        {
            dest = lab;
        }

        internal override string GetInstName()
        {
            return "" + (BranchOp)instr;
        }

        internal void MakeTargetLabel(ArrayList labs)
        {
            uint targetOffset = (uint)(offset + size + target);
            dest = CILInstructions.GetLabel(labs, targetOffset);
        }

        internal sealed override bool Check(MetaDataOut md)
        {
            target = (int)dest.GetLabelOffset() - (int)(offset + size);
            if((target < minByteVal) || (target > maxByteVal)) { // check for longver
                if(shortVer) {
                    if(instr == (uint)BranchOp.leave_s)
                        instr = (uint)BranchOp.leave;
                    else
                        instr = instr += longInstrOffset;
                    size += 3;
                    shortVer = false;
                    return true;
                }
            } else if(!shortVer) { // check for short ver
                if(instr == (uint)BranchOp.leave)
                    instr = (uint)BranchOp.leave_s;
                else
                    instr = instr -= longInstrOffset;
                size -= 3;
                shortVer = true;
                return true;
            }
            /*
            if (shortVer && ((target < minByteVal) || (target > maxByteVal))) {
              if (instr < (int)BranchOp.leave) instr += longInstrOffset;
              else instr--;
              shortVer = false;
              size += 3;
              return true;
            }
            */
            return false;
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            if(shortVer)
                output.Write((sbyte)target);
            else
                output.Write(target);
        }

    }

    /**************************************************************************/
    public class SwitchInstr : Instr {
        CILLabel[] cases;
        uint numCases = 0;
        int[] targets;

        /*-------------------- Constructors ---------------------------------*/

        public SwitchInstr(CILLabel[] dsts)
            : base(0x45)
        {
            cases = dsts;
            if(cases != null)
                numCases = (uint)cases.Length;
            size += 4 + (numCases * 4);
        }

        internal SwitchInstr(int[] offsets)
            : base(0x45)
        {
            numCases = (uint)offsets.Length;
            targets = offsets;
            size += 4 + (numCases * 4);
        }

        public CILLabel[] GetDests()
        {
            return cases;
        }
        public void SetDests(CILLabel[] dests)
        {
            cases = dests;
        }

        internal override string GetInstName()
        {
            return "switch";
        }

        internal void MakeTargetLabels(ArrayList labs)
        {
            cases = new CILLabel[numCases];
            for(int i=0; i < numCases; i++) {
                cases[i] = CILInstructions.GetLabel(labs, (uint)(offset + targets[i]));
            }
        }

        internal sealed override void Write(PEWriter output)
        {
            base.Write(output);
            output.Write(numCases);
            for(int i=0; i < numCases; i++) {
                int target = (int)cases[i].GetLabelOffset() - (int)(offset + size);
                output.Write(target);
            }
        }

    }

    /**************************************************************************/
    internal enum EHClauseType {
        Exception,
        Filter,
        Finally,
        Fault=4
    }

    internal class EHClause {
        EHClauseType clauseType;
        uint tryOffset, tryLength, handlerOffset, handlerLength, filterOffset = 0;
        MetaDataElement classToken = null;

        internal EHClause(EHClauseType cType, uint tOff, uint tLen, uint hOff, uint hLen)
        {
            clauseType = cType;
            tryOffset = tOff;
            tryLength = tLen;
            handlerOffset = hOff;
            handlerLength = hLen;
        }

        internal void ClassToken(MetaDataElement cToken)
        {
            classToken = cToken;
        }

        internal void FilterOffset(uint fOff)
        {
            filterOffset = fOff;
        }

        internal TryBlock MakeTryBlock(ArrayList labels)
        {
            TryBlock tBlock = new TryBlock(CILInstructions.GetLabel(labels, tryOffset),
              CILInstructions.GetLabel(labels, tryOffset + tryLength));
            CILLabel hStart = CILInstructions.GetLabel(labels, handlerOffset);
            CILLabel hEnd = CILInstructions.GetLabel(labels, handlerOffset+handlerLength);
            HandlerBlock handler = null;
            switch(clauseType) {
            case (EHClauseType.Exception):
                handler = new Catch((Class)classToken, hStart, hEnd);
                break;
            case (EHClauseType.Filter):
                handler = new Filter(CILInstructions.GetLabel(labels, filterOffset), hStart, hEnd);
                break;
            case (EHClauseType.Finally):
                handler = new Finally(hStart, hEnd);
                break;
            case (EHClauseType.Fault):
                handler = new Fault(hStart, hEnd);
                break;
            }
            tBlock.AddHandler(handler);
            return tBlock;
        }

    }

    /**************************************************************************/
    public abstract class CodeBlock {
        private static readonly int maxCodeSize = 255;
        protected CILLabel start, end;
        protected bool small = true;

        /*-------------------- Constructors ---------------------------------*/

        public CodeBlock(CILLabel start, CILLabel end)
        {
            this.start = start;
            this.end = end;
        }

        internal virtual bool isFat()
        {
            // Console.WriteLine("block start = " + start.GetLabelOffset() +
            //                  "  block end = " + end.GetLabelOffset());
            return (end.GetLabelOffset() - start.GetLabelOffset()) > maxCodeSize;
        }

        internal virtual void Write(PEWriter output, bool fatFormat)
        {
            if(fatFormat)
                output.Write(start.GetLabelOffset());
            else
                output.Write((short)start.GetLabelOffset());
            uint len = end.GetLabelOffset() - start.GetLabelOffset();
            if(Diag.DiagOn)
                Console.WriteLine("block start = " + start.GetLabelOffset() + "  len = " + len);
            if(fatFormat)
                output.Write(len);
            else
                output.Write((byte)len);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// The descriptor for a guarded block (.try)
    /// </summary>
    public class TryBlock : CodeBlock {
        protected bool fatFormat = false;
        protected ushort flags = 0;
        ArrayList handlers = new ArrayList();

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new try block
        /// </summary>
        /// <param name="start">start label for the try block</param>
        /// <param name="end">end label for the try block</param>
        public TryBlock(CILLabel start, CILLabel end)
            : base(start, end)
        {
        }

        /// <summary>
        /// Add a handler to this try block
        /// </summary>
        /// <param name="handler">a handler to be added to the try block</param>
        public void AddHandler(HandlerBlock handler)
        {
            //flags = handler.GetFlag();
            handlers.Add(handler);
        }

        internal void SetSize()
        {
            fatFormat = base.isFat();
            if(fatFormat)
                return;
            for(int i=0; i < handlers.Count; i++) {
                HandlerBlock handler = (HandlerBlock)handlers[i];
                if(handler.isFat()) {
                    fatFormat = true;
                    return;
                }
            }
        }

        internal int NumHandlers()
        {
            return handlers.Count;
        }

        internal override bool isFat()
        {
            return fatFormat;
        }

        internal void BuildTables(MetaDataOut md)
        {
            for(int i=0; i < handlers.Count; i++) {
                ((HandlerBlock)handlers[i]).BuildTables(md);
            }
        }

        internal override void Write(PEWriter output, bool fatFormat)
        {
            if(Diag.DiagOn)
                Console.WriteLine("writing exception details");
            for(int i=0; i < handlers.Count; i++) {
                if(Diag.DiagOn)
                    Console.WriteLine("Except block " + i);
                HandlerBlock handler = (HandlerBlock)handlers[i];
                flags = handler.GetFlag();
                if(Diag.DiagOn)
                    Console.WriteLine("flags = " + flags);
                if(fatFormat)
                    output.Write((uint)flags);
                else
                    output.Write(flags);
                base.Write(output, fatFormat);
                handler.Write(output, fatFormat);
            }
        }
    }

    /**************************************************************************/
    public abstract class HandlerBlock : CodeBlock {
        protected static readonly ushort ExceptionFlag = 0;
        protected static readonly ushort FilterFlag = 0x01;
        protected static readonly ushort FinallyFlag = 0x02;
        protected static readonly ushort FaultFlag = 0x04;

        /*-------------------- Constructors ---------------------------------*/

        public HandlerBlock(CILLabel start, CILLabel end)
            : base(start, end)
        {
        }

        internal virtual ushort GetFlag()
        {
            if(Diag.DiagOn)
                Console.WriteLine("Catch Block");
            return ExceptionFlag;
        }

        internal virtual void BuildTables(MetaDataOut md)
        {
        }

        internal override void Write(PEWriter output, bool fatFormat)
        {
            base.Write(output, fatFormat);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// The descriptor for a catch clause (.catch)
    /// </summary>
    public class Catch : HandlerBlock {
        Class exceptType;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new catch clause
        /// </summary>
        /// <param name="except">the exception to be caught</param>
        /// <param name="handlerStart">start of the handler code</param>
        /// <param name="handlerEnd">end of the handler code</param>
        public Catch(Class except, CILLabel handlerStart, CILLabel handlerEnd)
            : base(handlerStart, handlerEnd)
        {
            exceptType = except;
        }

        internal override void BuildTables(MetaDataOut md)
        {
            if(!(exceptType is ClassDef))
                exceptType.BuildMDTables(md);
        }

        internal override void Write(PEWriter output, bool fatFormat)
        {
            base.Write(output, fatFormat);
            output.Write(exceptType.Token());
        }
    }

    /**************************************************************************/
    /// <summary>
    /// The descriptor for a filter clause (.filter)
    /// </summary>
    public class Filter : HandlerBlock {
        CILLabel filterLabel;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new filter clause
        /// </summary>
        /// <param name="filterLabel">the label where the filter code starts</param>
        /// <param name="handlerStart">the start of the handler code</param>
        /// <param name="handlerEnd">the end of the handler code</param>
        public Filter(CILLabel filterLabel, CILLabel handlerStart,
          CILLabel handlerEnd)
            : base(handlerStart, handlerEnd)
        {
            this.filterLabel = filterLabel;
        }

        internal override ushort GetFlag()
        {
            if(Diag.DiagOn)
                Console.WriteLine("Filter Block");
            return FilterFlag;
        }

        internal override void Write(PEWriter output, bool fatFormat)
        {
            base.Write(output, fatFormat);
            output.Write(filterLabel.GetLabelOffset());
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for a finally block (.finally)
    /// </summary>
    public class Finally : HandlerBlock {

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new finally clause
        /// </summary>
        /// <param name="finallyStart">start of finally code</param>
        /// <param name="finallyEnd">end of finally code</param>
        public Finally(CILLabel finallyStart, CILLabel finallyEnd)
            : base(finallyStart, finallyEnd)
        {
        }

        internal override ushort GetFlag()
        {
            if(Diag.DiagOn)
                Console.WriteLine("Finally Block");
            return FinallyFlag;
        }

        internal override void Write(PEWriter output, bool fatFormat)
        {
            base.Write(output, fatFormat);
            output.Write((int)0);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for a fault block (.fault)
    /// </summary>
    public class Fault : HandlerBlock {

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new fault clause
        /// </summary>
        /// <param name="faultStart">start of the fault code</param>
        /// <param name="faultEnd">end of the fault code</param>
        public Fault(CILLabel faultStart, CILLabel faultEnd)
            : base(faultStart, faultEnd)
        {
        }

        internal override ushort GetFlag()
        {
            if(Diag.DiagOn)
                Console.WriteLine("Fault Block");
            return FaultFlag;
        }

        internal override void Write(PEWriter output, bool fatFormat)
        {
            base.Write(output, fatFormat);
            output.Write((int)0);

        }
    }

    /**************************************************************************/
    // Classes used to describe constant values
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a constant value, to be written in the blob heap
    /// </summary>
    public abstract class Constant {
        protected uint size = 0;
        internal ElementType type;
        protected uint blobIndex;
        internal MetaDataOut addedToBlobHeap;

        /*-------------------- Constructors ---------------------------------*/

        internal Constant()
        {
        }

        internal virtual uint GetBlobIndex(MetaDataOut md)
        {
            return 0;
        }

        internal uint GetSize()
        {
            return size;
        }

        internal byte GetTypeIndex()
        {
            return (byte)type;
        }

        internal virtual void Write(BinaryWriter bw)
        {
        }

    }

    /**************************************************************************/
    public abstract class BlobConstant : Constant {
    }
    /**************************************************************************/
    /// <summary>
    /// Boolean constant
    /// </summary>
    public class BoolConst : BlobConstant {
        bool val;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new boolean constant with the value "val"
        /// </summary>
        /// <param name="val">value of this boolean constant</param>
        public BoolConst(bool val)
        {
            this.val = val;
            size = 1;
            type = ElementType.Boolean;
        }

        public bool GetBool()
        {
            return val;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                if(val)
                    blobIndex = md.AddToBlobHeap(1, 1);
                else
                    blobIndex = md.AddToBlobHeap(0, 1);
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            if(val)
                bw.Write((sbyte)1);
            else
                bw.Write((sbyte)0);
        }

    }
    /**************************************************************************/
    public class CharConst : BlobConstant {
        char val;

        /*-------------------- Constructors ---------------------------------*/

        public CharConst(char val)
        {
            this.val = val;
            size = 2;
            type = ElementType.Char;
        }

        internal CharConst(PEReader buff)
        {
            val = buff.ReadChar();
            size = 2;
            type = ElementType.Char;
        }

        public char GetChar()
        { // KJG addition 2005-Mar-01
            return val;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                blobIndex = md.AddToBlobHeap(val);
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            bw.Write(val);
        }

    }

    /**************************************************************************/
    public class NullRefConst : BlobConstant {

        /*-------------------- Constructors ---------------------------------*/

        public NullRefConst()
        {
            size = 4;
            type = ElementType.Class;
        }

        internal NullRefConst(PEReader buff)
        {
            uint junk = buff.ReadUInt32();
            size = 4;
            type = ElementType.Class;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                blobIndex = md.AddToBlobHeap(0, 4);
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            bw.Write((int)0);
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Constant array
    /// </summary>
    public class ArrayConst : BlobConstant {
        Constant[] elements;

        /*-------------------- Constructors ---------------------------------*/

        public ArrayConst(Constant[] elems)
        {
            type = ElementType.SZArray;
            size = 5;  // one byte for SZARRAY, 4 bytes for length
            elements = elems;
            for(int i=0; i < elements.Length; i++) {
                size += elements[i].GetSize();
            }
        }

        public Constant[] GetArray()
        {
            return elements;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                MemoryStream str = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(str);
                Write(bw);
                blobIndex = md.AddToBlobHeap(str.ToArray());
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            bw.Write((byte)type);
            bw.Write(elements.Length);
            for(int i=0; i < elements.Length; i++) {
                elements[i].Write(bw);
            }
        }

    }

    /**************************************************************************/
    public class ClassTypeConst : BlobConstant {
        string name;
        Class desc;

        /*-------------------- Constructors ---------------------------------*/

        public ClassTypeConst(string className)
        {
            name = className;
            type = ElementType.ClassType;
        }

        public ClassTypeConst(Class classDesc)
        {
            desc = classDesc;
            type = ElementType.ClassType;
        }

        public Class GetClass()
        {
            return desc;
        }

        public String GetClassName()
        {
            if(name == null)
                name = desc.TypeName();
            // CHECK - ClassName or TypeName
            // if (name == null) return desc.ClassName();
            return name;
        }

        internal override void Write(BinaryWriter bw)
        {
            if(name == null)
                name = desc.TypeName();
            // CHECK - ClassName or TypeName
            // if (name == null)  name = desc.ClassName();
            bw.Write(name);
        }

    }

    /**************************************************************************/
    public class BoxedSimpleConst : BlobConstant {
        SimpleConstant sConst;

        /*-------------------- Constructors ---------------------------------*/

        public BoxedSimpleConst(SimpleConstant con)
        {
            sConst = con;
            type = (ElementType)sConst.GetTypeIndex();
        }

        public SimpleConstant GetConst()
        {
            return sConst;
        }

        internal override void Write(BinaryWriter bw)
        {
            bw.Write((byte)type);
            sConst.Write(bw);
        }
    }
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a constant value
    /// </summary>
    public abstract class DataConstant : Constant {
        private uint dataOffset = 0;

        /*-------------------- Constructors ---------------------------------*/

        internal DataConstant()
        {
        }

        public uint DataOffset
        {
            get
            {
                return dataOffset;
            }
            set
            {
                dataOffset = value;
            }
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Constant for a memory address
    /// </summary>
    public class AddressConstant : DataConstant {
        DataConstant data;

        /*-------------------- Constructors ---------------------------------*/

        public AddressConstant(DataConstant dConst)
        {
            data = dConst;
            size = 4;
            type = ElementType.TypedByRef;
        }

        internal AddressConstant(PEReader buff)
        {
        }

        public DataConstant GetConst()
        {
            return data;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            ((PEWriter)bw).WriteDataRVA(data.DataOffset);
        }

    }

    /**************************************************************************/
    public class ByteArrConst : DataConstant {
        byte[] val;

        /*-------------------- Constructors ---------------------------------*/

        public ByteArrConst(byte[] val)
        {
            this.val = val;
            size = (uint)val.Length;
        }

        public byte[] GetArray()
        {
            return val;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                blobIndex = md.AddToBlobHeap(val);
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            bw.Write(val);
        }

    }

    /**************************************************************************/
    public class RepeatedConstant : DataConstant {
        DataConstant data;
        uint repCount;

        /*-------------------- Constructors ---------------------------------*/

        public RepeatedConstant(DataConstant dConst, int repeatCount)
        {
            data = dConst;
            repCount = (uint)repeatCount;
            type = ElementType.SZArray;
            size = data.GetSize() * repCount;
        }

        public DataConstant GetConst()
        {
            return data;
        }

        public uint GetCount()
        {
            return repCount;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            for(int i=0; i < repCount; i++) {
                data.Write(bw);
            }
        }

    }

    /**************************************************************************/
    public class StringConst : DataConstant {
        string val;
        byte[] strBytes;

        /*-------------------- Constructors ---------------------------------*/

        public StringConst(string val)
        {
            this.val = val;
            size = (uint)val.Length;  // need to add null ??
            type = ElementType.String;
        }

        internal StringConst(byte[] sBytes)
        {
            strBytes = sBytes;
            size = (uint)strBytes.Length;
            type = ElementType.String;
        }

        public string GetString()
        {
            return val;
        }

        public byte[] GetStringBytes()
        {
            return strBytes;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                if(val == null)
                    blobIndex = md.AddToBlobHeap(strBytes);
                else
                    blobIndex = md.AddToBlobHeap(val);
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            if((val == null) && (strBytes != null)) {
                bw.Write(strBytes);
            } else
                bw.Write(val);
        }

    }

    /**************************************************************************/
    public abstract class SimpleConstant : DataConstant {
    }

    /**************************************************************************/
    public class IntConst : SimpleConstant {
        long val;

        /*-------------------- Constructors ---------------------------------*/

        public IntConst(sbyte val)
        {
            this.val = val;
            size = 1; //8;
            type = ElementType.I8;
        }

        public IntConst(short val)
        {
            this.val = val;
            size = 2; //16;
            type = ElementType.I2;
        }

        public IntConst(int val)
        {
            this.val = val;
            size = 4; //32;
            type = ElementType.I4;
        }

        public IntConst(long val)
        {
            this.val = val;
            size = 8; //64;
            type = ElementType.I8;
        }

        internal IntConst(PEReader buff, int numBytes)
        {
            switch(numBytes) {
            case (1):
                val = buff.ReadSByte();
                type = ElementType.I8;
                break;
            case (2):
                val = buff.ReadInt16();
                type = ElementType.I2;
                break;
            case (4):
                val = buff.ReadInt32();
                type = ElementType.I4;
                break;
            case (8):
                val = buff.ReadInt64();
                type = ElementType.I8;
                break;
            default:
                val = 0;
                break;
            }
            size = (uint)numBytes; // * 4;
        }

        public int GetIntSize()
        {
            return (int)size;
        }

        public ElementType GetIntType()
        {
            return type;
        }

        public int GetInt()
        {
            if(size < 8)
                return (int)val;
            else
                throw new Exception("Constant is long");
        }

        public long GetLong()
        {
            return val;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                blobIndex = md.AddToBlobHeap(val, size);
                //switch (size) {
                //  case (1) : md.AddToBlobHeap((sbyte)val); break;
                //  case (2) : md.AddToBlobHeap((short)val); break;
                //  case (4) : md.AddToBlobHeap((int)val); break;
                //  default : md.AddToBlobHeap(val); break; 
                //}
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            switch(size) {
            case (1):
                bw.Write((sbyte)val);
                break;
            case (2):
                bw.Write((short)val);
                break;
            case (4):
                bw.Write((int)val);
                break;
            default:
                bw.Write(val);
                break;
            }
        }

    }

    /**************************************************************************/
    public class UIntConst : SimpleConstant {
        ulong val;

        /*-------------------- Constructors ---------------------------------*/

        public UIntConst(byte val)
        {
            this.val = val;
            size = 1;
            type = ElementType.U8;
        }
        public UIntConst(ushort val)
        {
            this.val = val;
            size = 2;
            type = ElementType.U2;
        }
        public UIntConst(uint val)
        {
            this.val = val;
            size = 4;
            type = ElementType.U4;
        }
        public UIntConst(ulong val)
        {
            this.val = val;
            size = 8;
            type = ElementType.U8;
        }

        public int GetIntSize()
        {
            return (int)size;
        }

        public ElementType GetUIntType()
        {
            return type;
        }

        public uint GetUInt()
        {
            return (uint)val;
        }

        public ulong GetULong()
        {
            return val;
        }

        public long GetLong()
        {           // KJG addition
            if(val <= (ulong)(System.Int64.MaxValue))
                return (long)val;
            else
                throw new Exception("UInt Constant too large");
        }

        public long GetULongAsLong()
        {   // KJG addition
            return (long)val;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                blobIndex = md.AddToBlobHeap(val, size);
                //switch (size) {
                //  case (1) : blobIndex = md.AddToBlobHeap((byte)val); break;
                //  case (2) : blobIndex = md.AddToBlobHeap((ushort)val); break;
                //  case (4) : blobIndex = md.AddToBlobHeap((uint)val); break;
                //  default : blobIndex = md.AddToBlobHeap(val); break;
                //}
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            switch(size) {
            case (1):
                bw.Write((byte)val);
                break;
            case (2):
                bw.Write((ushort)val);
                break;
            case (4):
                bw.Write((uint)val);
                break;
            default:
                bw.Write(val);
                break;
            }
        }
    }

    /**************************************************************************/
    public class FloatConst : SimpleConstant {
        float val;

        /*-------------------- Constructors ---------------------------------*/

        public FloatConst(float val)
        {
            this.val = val;
            size = 4;
            type = ElementType.R4;
        }

        internal FloatConst(PEReader buff)
        {
            val = buff.ReadSingle();
            size = 4;
            type = ElementType.R4;
        }

        public float GetFloat()
        {
            return val;
        }

        public double GetDouble()
        { // KJG addition 2005-Mar-01
            return (double)val;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                blobIndex = md.AddToBlobHeap(val);
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            bw.Write(val);
        }

    }

    /**************************************************************************/
    public class DoubleConst : SimpleConstant {
        double val;

        /*-------------------- Constructors ---------------------------------*/

        public DoubleConst(double val)
        {
            this.val = val;
            size = 8;
            type = ElementType.R8;
        }

        internal DoubleConst(PEReader buff)
        {
            val = buff.ReadDouble();
            size = 8;
            type = ElementType.R8;
        }

        public double GetDouble()
        { // KJG addition 2005-Mar-01
            return val;
        }

        internal sealed override uint GetBlobIndex(MetaDataOut md)
        {
            if(addedToBlobHeap != md) {
                blobIndex = md.AddToBlobHeap(val);
                addedToBlobHeap = md;
            }
            return blobIndex;
        }

        internal sealed override void Write(BinaryWriter bw)
        {
            bw.Write(val);
        }

    }
    /**************************************************************************/
    // Class to describe procedure locals
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a local of a method
    /// </summary>
    public class Local {
        private static readonly byte PINNED = 0x45;
        string name;
        Type type;
        bool pinned = false;
        int index = 0;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new local variable 
        /// </summary>
        /// <param name="lName">name of the local variable</param>
        /// <param name="lType">type of the local variable</param>
        public Local(string lName, Type lType)
        {
            name = lName;
            type = lType;
        }

        /// <summary>
        /// Create a new local variable that is byref and/or pinned
        /// </summary>
        /// <param name="lName">local name</param>
        /// <param name="lType">local type</param>
        /// <param name="isPinned">has pinned attribute</param>
        public Local(string lName, Type lType, bool isPinned)
        {
            name = lName;
            type = lType;
            pinned = isPinned;
        }

        public int GetIndex()
        {
            return index;
        }

        public bool Pinned
        {
            get
            {
                return pinned;
            }
            set
            {
                pinned = value;
            }
        }

        internal void SetIndex(int ix)
        {
            index = ix;
        }

        internal void TypeSig(MemoryStream str)
        {
            if(pinned)
                str.WriteByte(PINNED);
            type.TypeSig(str);
        }

        internal void BuildTables(MetaDataOut md)
        {
            if(!(type is ClassDef))
                type.BuildMDTables(md);
        }

    }
    /**************************************************************************/
    // Class of PEFile constant values
    /**************************************************************************/
    /// <summary>
    /// Image for a PEFile
    /// File Structure
    ///     DOS Header (128 bytes) 
    ///     PE Signature ("PE\0\0") 
    ///     PEFileHeader (20 bytes)
    ///     PEOptionalHeader (224 bytes) 
    ///     SectionHeaders (40 bytes * NumSections)
    ///
    ///     Sections .text (always present - contains metadata)
    ///              .sdata (contains any initialised data in the file - may not be present)
    ///                     (for ilams /debug this contains the Debug table)
    ///              .reloc (always present - in pure CIL only has one fixup)
    ///               others???  c# produces .rsrc section containing a Resource Table
    ///
    /// .text layout
    ///     IAT (single entry 8 bytes for pure CIL)
    ///     CLIHeader (72 bytes)
    ///     CIL instructions for all methods (variable size)
    ///     MetaData 
    ///       Root (20 bytes + UTF-8 Version String + quad align padding)
    ///       StreamHeaders (8 bytes + null terminated name string + quad align padding)
    ///       Streams 
    ///         #~        (always present - holds metadata tables)
    ///         #Strings  (always present - holds identifier strings)
    ///         #US       (Userstring heap)
    ///         #Blob     (signature blobs)
    ///         #GUID     (guids for assemblies or Modules)
    ///    ImportTable (40 bytes)
    ///    ImportLookupTable(8 bytes) (same as IAT for standard CIL files)
    ///    Hint/Name Tables with entry "_CorExeMain" for .exe file and "_CorDllMain" for .dll (14 bytes)
    ///    ASCII string "mscoree.dll" referenced in ImportTable (+ padding = 16 bytes)
    ///    Entry Point  (0xFF25 followed by 4 bytes 0x400000 + RVA of .text)
    ///
    ///  #~ stream structure
    ///    Header (24 bytes)
    ///    Rows   (4 bytes * numTables)
    ///    Tables
    /// </summary>
    internal class FileImage {
        internal readonly static uint DelaySignSize = 128; // Current assemblies are always 128
        internal readonly static uint[] iByteMask = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };
        internal readonly static ulong[] lByteMask = {0x00000000000000FF, 0x000000000000FF00,
                                                   0x0000000000FF0000, 0x00000000FF000000,
                                                   0x000000FF00000000, 0x0000FF0000000000,
                                                   0x00FF000000000000, 0xFF00000000000000 };
        internal readonly static uint nibble0Mask = 0x0000000F;
        internal readonly static uint nibble1Mask = 0x000000F0;

        internal static readonly byte[] DOSHeader = { 0x4d,0x5a,0x90,0x00,0x03,0x00,0x00,0x00,
                                                  0x04,0x00,0x00,0x00,0xff,0xff,0x00,0x00,
                                                  0xb8,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                  0x40,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                  0x00,0x00,0x00,0x00,0x80,0x00,0x00,0x00,
                                                  0x0e,0x1f,0xba,0x0e,0x00,0xb4,0x09,0xcd,
                                                  0x21,0xb8,0x01,0x4c,0xcd,0x21,0x54,0x68,
                                                  0x69,0x73,0x20,0x70,0x72,0x6f,0x67,0x72,
                                                  0x61,0x6d,0x20,0x63,0x61,0x6e,0x6e,0x6f,
                                                  0x74,0x20,0x62,0x65,0x20,0x72,0x75,0x6e,
                                                  0x20,0x69,0x6e,0x20,0x44,0x4f,0x53,0x20,
                                                  0x6d,0x6f,0x64,0x65,0x2e,0x0d,0x0d,0x0a,
                                                  0x24,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                                                  0x50,0x45,0x00,0x00};
        internal static readonly int PESigOffset = 0x3C;
        internal static byte[] PEHeader = { 0x4c, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0xE0, 0x00, 0x0E, 0x01, // PE Header Standard Fields
                                        0x0B, 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                                      };

        internal static IType[] instrMap = { 
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x00 - 0x08
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.uint8Op, IType.uint8Op, // 0x09 - 0x0F
                                         IType.uint8Op, IType.uint8Op, IType.uint8Op, IType.uint8Op,   // 0x10 - 0x13
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x14 - 0x1C
                                         IType.op, IType.op, IType.int8Op, IType.int32Op, IType.specialOp,   // 0x1D - 0x21
                                         IType.specialOp, IType.specialOp,IType.op,IType.op,IType.op,IType.methOp, // 0x22 - 0x27
                                         IType.methOp, IType.specialOp, IType.op, IType.branchOp, IType.branchOp,// 0x28 - 0x2C
                                         IType.branchOp, IType.branchOp, IType.branchOp, IType.branchOp,       // 0x2D - 0x30
                                         IType.branchOp, IType.branchOp, IType.branchOp, IType.branchOp,       // 0x31 - 0x34
                                         IType.branchOp, IType.branchOp, IType.branchOp, IType.branchOp,      // 0x35 - 0x38
                                         IType.branchOp, IType.branchOp, IType.branchOp, IType.branchOp,   // 0x39 - 0x3C
                                         IType.branchOp, IType.branchOp, IType.branchOp, IType.branchOp,   // 0x3D - 0x40
                                         IType.branchOp, IType.branchOp, IType.branchOp, IType.branchOp,   // 0x41 - 0x44
                                         IType.specialOp, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,    // 0x45 - 0x4B
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x4C - 0x54
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x55 - 0x5D
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x5E - 0x66
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,       // 0x67 - 0x6E
                                         IType.methOp, IType.typeOp, IType.typeOp, IType.specialOp,     // 0x6F - 0x72
                                         IType.methOp, IType.typeOp, IType.typeOp, IType.op, IType.op, IType.op,   // 0x73 - 0x78
                                         IType.typeOp, IType.op, IType.fieldOp, IType.fieldOp, IType.fieldOp,// 0x79 - 0x7D
                                         IType.fieldOp, IType.fieldOp, IType.fieldOp, IType.typeOp,    // 0x7E - 0x81
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x82 - 0x8A
                                         IType.op, IType.typeOp, IType.typeOp, IType.op, IType.typeOp, IType.op,   // 0x8B - 0x90
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x91 - 0x99
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0x9A - 0xA2
                                         IType.typeOp, IType.typeOp, IType.typeOp, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0xA3 - 0xAB
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0xAC - 0xB4
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0xB5 - 0xBD
                                         IType.op, IType.op, IType.op, IType.op, IType.typeOp, IType.op, IType.op, IType.op,   // 0xBE - 0xC5
                                         IType.typeOp, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,   // 0xC6 - 0xCD
                                         IType.op, IType.op, IType.specialOp, IType.op, IType.op, IType.op, IType.op,    // 0xCE - 0xD4
                                         IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.op,       // 0xD5 - 0xDC
                                         IType.branchOp, IType.branchOp, IType.op, IType.op };                            // 0xDD - 0xE0

        internal static IType[] longInstrMap = { IType.op, IType.op, IType.op, IType.op, IType.op, IType.op, IType.methOp,   // 0x00 - 0x06
                                             IType.methOp, IType.uint16Op, IType.uint16Op,       // 0x07 - 0x09
                                             IType.uint16Op, IType.uint16Op, IType.uint16Op,     // 0x0A - 0x0C
                                             IType.uint16Op, IType.uint16Op, IType.op, IType.op, IType.op,   // 0x0D - 0x11
                                             IType.uint8Op, IType.op, IType.op, IType.typeOp, IType.typeOp, IType.op,  // 0x12 - 0x17
                                             IType.op, IType.op, IType.op, IType.op, IType.typeOp, IType.op, IType.op};  // 0x18 - 0x1D

        internal static readonly uint bitmask0  = 0x00000001;
        internal static readonly uint bitmask1  = 0x00000002;
        internal static readonly uint bitmask2  = 0x00000004;
        internal static readonly uint bitmask3  = 0x00000008;
        internal static readonly uint bitmask4  = 0x00000010;
        internal static readonly uint bitmask5  = 0x00000020;
        internal static readonly uint bitmask6  = 0x00000040;
        internal static readonly uint bitmask7  = 0x00000080;
        internal static readonly uint bitmask8  = 0x00000100;
        internal static readonly uint bitmask9  = 0x00000200;
        internal static readonly uint bitmask10 = 0x00000400;
        internal static readonly uint bitmask11 = 0x00000800;
        internal static readonly uint bitmask12 = 0x00001000;
        internal static readonly uint bitmask13 = 0x00002000;
        internal static readonly uint bitmask14 = 0x00004000;
        internal static readonly uint bitmask15 = 0x00008000;
        internal static readonly uint bitmask16 = 0x00010000;
        internal static readonly uint bitmask17 = 0x00020000;
        internal static readonly uint bitmask18 = 0x00040000;
        internal static readonly uint bitmask19 = 0x00080000;
        internal static readonly uint bitmask20 = 0x00100000;
        internal static readonly uint bitmask21 = 0x00200000;
        internal static readonly uint bitmask22 = 0x00400000;
        internal static readonly uint bitmask23 = 0x00800000;
        internal static readonly uint bitmask24 = 0x01000000;
        internal static readonly uint bitmask25 = 0x02000000;
        internal static readonly uint bitmask26 = 0x04000000;
        internal static readonly uint bitmask27 = 0x08000000;
        internal static readonly uint bitmask28 = 0x10000000;
        internal static readonly uint bitmask29 = 0x20000000;
        internal static readonly uint bitmask30 = 0x40000000;
        internal static readonly uint bitmask31 = 0x80000000;
        internal static readonly ulong bitmask32 = 0x0000000100000000;
        internal static readonly ulong bitmask33 = 0x0000000200000000;
        internal static readonly ulong bitmask34 = 0x0000000400000000;
        internal static readonly ulong bitmask35 = 0x0000000800000000;
        internal static readonly ulong bitmask36 = 0x0000001000000000;
        internal static readonly ulong bitmask37 = 0x0000002000000000;
        internal static readonly ulong bitmask38 = 0x0000004000000000;
        internal static readonly ulong bitmask39 = 0x0000008000000000;
        internal static readonly ulong bitmask40 = 0x0000010000000000;
        internal static readonly ulong bitmask41 = 0x0000020000000000;
        internal static readonly ulong bitmask42 = 0x0000040000000000;
        internal static readonly ulong bitmask43 = 0x0000080000000000;
        internal static readonly ulong bitmask44 = 0x0000100000000000;
        internal static readonly ulong bitmask45 = 0x0000200000000000;
        internal static readonly ulong bitmask46 = 0x0000400000000000;
        internal static readonly ulong bitmask47 = 0x0000800000000000;
        internal static readonly ulong bitmask48 = 0x0001000000000000;
        internal static readonly ulong bitmask49 = 0x0002000000000000;
        internal static readonly ulong bitmask50 = 0x0004000000000000;
        internal static readonly ulong bitmask51 = 0x0008000000000000;
        internal static readonly ulong bitmask52 = 0x0010000000000000;
        internal static readonly ulong bitmask53 = 0x0020000000000000;
        internal static readonly ulong bitmask54 = 0x0040000000000000;
        internal static readonly ulong bitmask55 = 0x0080000000000000;
        internal static readonly ulong bitmask56 = 0x0100000000000000;
        internal static readonly ulong bitmask57 = 0x0200000000000000;
        internal static readonly ulong bitmask58 = 0x0400000000000000;
        internal static readonly ulong bitmask59 = 0x0800000000000000;
        internal static readonly ulong bitmask60 = 0x1000000000000000;
        internal static readonly ulong bitmask61 = 0x2000000000000000;
        internal static readonly ulong bitmask62 = 0x4000000000000000;
        internal static readonly ulong bitmask63 = 0x8000000000000000;

        internal static readonly ulong[] bitmasks = { bitmask0 , bitmask1 , bitmask2 , bitmask3 ,
                                                  bitmask4 , bitmask5 , bitmask6 , bitmask7 ,
                                                  bitmask8 , bitmask9 , bitmask10, bitmask11,
                                                  bitmask12, bitmask13, bitmask14, bitmask15,
                                                  bitmask16, bitmask17, bitmask18, bitmask19,
                                                  bitmask20, bitmask21, bitmask22, bitmask23,
                                                  bitmask24, bitmask25, bitmask26, bitmask27,
                                                  bitmask28, bitmask29, bitmask30, bitmask31,
                                                  bitmask32, bitmask33, bitmask34, bitmask35,
                                                  bitmask36, bitmask37, bitmask38, bitmask39,
                                                  bitmask40, bitmask41, bitmask42, bitmask43,
                                                  bitmask44, bitmask45, bitmask46, bitmask47,
                                                  bitmask48, bitmask49, bitmask50, bitmask51,
                                                  bitmask52, bitmask53, bitmask54, bitmask55,
                                                  bitmask56, bitmask57, bitmask58, bitmask59,
                                                  bitmask60, bitmask61, bitmask62, bitmask63 };


        internal static readonly uint TableMask = 0xFF000000;
        internal static readonly uint ElementMask = 0x00FFFFFF;
        internal static readonly int NAMELEN = 8, STRLEN = 200;
        internal static readonly uint machine = 0x14C;
        internal static readonly uint magic = 0x10B;
        internal static readonly uint minFileAlign = 0x200;
        internal static readonly uint maxFileAlign = 0x1000;
        internal static readonly uint fileHeaderSize = 0x178;
        internal static readonly uint sectionHeaderSize = 40;
        internal static readonly uint SectionAlignment = 0x2000;
        internal static readonly uint ImageBase = 0x400000;
        internal static readonly uint ImportTableSize = 40;
        internal static readonly uint IATSize = 8;
        internal static readonly uint CLIHeaderSize = 72;
        internal static readonly uint relocFlags = 0x42000040;
        internal static readonly ushort exeCharacteristics = 0x010E;
        internal static readonly ushort dllCharacteristics = 0x210E;
        internal static readonly ushort dllFlag = 0x2000;
        // section names are all 8 bytes
        internal static readonly string textName = ".text\0\0\0";
        internal static readonly string sdataName = ".sdata\0\0";
        internal static readonly string relocName = ".reloc\0\0";
        internal static readonly string rsrcName = ".rsrc\0\0\0";
        internal static readonly string exeHintNameTable = "\0\0_CorExeMain\0";
        internal static readonly string dllHintNameTable = "\0\0_CorDllMain\0";
        internal static readonly string runtimeEngineName = "mscoree.dll\0\0";
        internal static readonly DateTime origin = new DateTime(1970, 1, 1);
        internal static readonly ushort DLLFlags = (ushort)0x400; // for ver 1.1.4322 prev = (short)0;
        internal static readonly uint StackReserveSize = 0x100000;
        internal static readonly uint StackCommitSize = 0x1000;
        internal static readonly uint HeapReserveSize = 0x100000;
        internal static readonly uint HeapCommitSize = 0x1000;
        internal static readonly uint LoaderFlags = 0;
        internal static readonly uint NumDataDirectories = 0x10;
    }
    /**************************************************************************/
    // Class to Write PE File
    /**************************************************************************/
    /*
     *internal class PDBWriter {
     *
     *  internal PDBWriter(string filename) { }
     *
     *  public byte[] GetDebugInfo() {
     *    return null;
     *  }
     * 
     *  public void WritePDBFile() {
     *  }
     *}
    */
    internal class PEWriter : BinaryWriter {
        private Section text, sdata, rsrc = null;
        ArrayList data;
        BinaryWriter reloc = new BinaryWriter(new MemoryStream());
        uint dateStamp = 0, codeStart = 0;
        uint numSections = 2; // always have .text  and .reloc sections
        internal PEFileVersionInfo verInfo;
        //internal bool delaySign;
        uint entryPointOffset, entryPointPadding, imageSize, headerSize, headerPadding, entryPointToken = 0;
        uint relocOffset, relocRVA, relocSize, relocPadding, relocTide, hintNameTableOffset, resourcesSize = 0;
        uint metaDataOffset, runtimeEngineOffset, initDataSize = 0, resourcesOffset, importTablePadding;
        uint importTableOffset, importLookupTableOffset, totalImportTableSize, entryPointReloc=0; //, delaySignOffset;
        uint /*debugOffset = 0,*/ debugSize = 0, debugRVA = 0;
        MetaDataOut metaData;
        char[] runtimeEngine = FileImage.runtimeEngineName.ToCharArray(), hintNameTable;
        bool closeStream = true;
        //byte[] debugBytes;
        //internal PDBWriter pdbWriter;

        /*-------------------- Constructors ---------------------------------*/

        internal PEWriter(PEFileVersionInfo verInfo, string fileName, MetaDataOut md, bool debug)
            : base(new FileStream(fileName, FileMode.Create))
        {
            InitPEWriter(verInfo, md, debug);
            TimeSpan tmp = System.IO.File.GetCreationTime(fileName).Subtract(FileImage.origin);
            dateStamp = Convert.ToUInt32(tmp.TotalSeconds);
        }

        internal PEWriter(PEFileVersionInfo verInfo, Stream str, MetaDataOut md, bool debug)
            : base(str)
        {
            InitPEWriter(verInfo, md, debug);
            TimeSpan tmp = DateTime.Now.Subtract(FileImage.origin);
            dateStamp = Convert.ToUInt32(tmp.TotalSeconds);
            closeStream = false;
        }



        /*----------------------------- Writing -----------------------------------------*/

        private void InitPEWriter(PEFileVersionInfo verInfo, MetaDataOut md, bool debug)
        {
            this.verInfo = verInfo;
            if(!verInfo.fromExisting)
                verInfo.lMajor = MetaData.LMajors[(int)verInfo.netVersion];
            if(verInfo.isDLL) {
                hintNameTable = FileImage.dllHintNameTable.ToCharArray();
                if(!verInfo.fromExisting)
                    verInfo.characteristics = FileImage.dllCharacteristics;
            } else {
                hintNameTable = FileImage.exeHintNameTable.ToCharArray();
                if(!verInfo.fromExisting)
                    verInfo.characteristics = FileImage.exeCharacteristics;
            }
            text = new Section(FileImage.textName, 0x60000020);     // IMAGE_SCN_CNT  CODE, EXECUTE, READ
            //			rsrc = new Section(rsrcName,0x40000040);     // IMAGE_SCN_CNT  INITIALIZED_DATA, READ
            metaData = md;
            metaData.InitMetaDataOut(this);
        }

        private uint GetNextSectStart(uint rva, uint tide)
        {
            if(tide < FileImage.SectionAlignment)
                return rva + FileImage.SectionAlignment;
            return rva + ((tide / FileImage.SectionAlignment) + 1) * FileImage.SectionAlignment;
        }

        private void BuildTextSection()
        {
            // .text layout
            //    IAT (single entry 8 bytes for pure CIL)
            //    CLIHeader (72 bytes)
            //    CIL instructions for all methods (variable size)
            //    Strong Name Signature
            //    MetaData 
            //    ManagedResources
            //    ImportTable (40 bytes)
            //    ImportLookupTable(8 bytes) (same as IAT for standard CIL files)
            //    Hint/Name Tables with entry "_CorExeMain" for .exe file and "_CorDllMain" for .dll (14 bytes)
            //    ASCII string "mscoree.dll" referenced in ImportTable (+ padding = 16 bytes)
            //    Entry Point  (0xFF25 followed by 4 bytes 0x400000 + RVA of .text)
            codeStart = FileImage.IATSize + FileImage.CLIHeaderSize;
            if(Diag.DiagOn)
                Console.WriteLine("Code starts at " + Hex.Int(codeStart));
            metaData.BuildMetaData();
            // strongNameSig = metaData.GetStrongNameSig();
            metaDataOffset = FileImage.IATSize + FileImage.CLIHeaderSize + metaData.CodeSize();
            //if (pdbWriter != null) {
            //  debugSize = 0x1C; // or size of debugBytes??
            //  debugOffset = metaDataOffset;
            //  metaDataOffset += (uint)debugBytes.Length + debugSize + NumToAlign((uint)debugBytes.Length,4);  
            //}
            resourcesOffset = metaDataOffset + metaData.Size();
            resourcesSize = metaData.GetResourcesSize();
            importTableOffset = resourcesOffset + resourcesSize;
            importTablePadding = NumToAlign(importTableOffset, 16);
            importTableOffset += importTablePadding;
            importLookupTableOffset = importTableOffset + FileImage.ImportTableSize;
            hintNameTableOffset = importLookupTableOffset + FileImage.IATSize;
            runtimeEngineOffset = hintNameTableOffset + (uint)hintNameTable.Length;
            entryPointOffset = runtimeEngineOffset + (uint)runtimeEngine.Length;
            totalImportTableSize = entryPointOffset - importTableOffset;
            if(Diag.DiagOn) {
                Console.WriteLine("total import table size = " + totalImportTableSize);
                Console.WriteLine("entrypoint offset = " + Hex.Int(entryPointOffset));
            }
            entryPointPadding = NumToAlign(entryPointOffset, 4) + 2;
            entryPointOffset += entryPointPadding;
            entryPointReloc = entryPointOffset + 2;
            text.IncTide(entryPointOffset + 6);
            if(text.Tide() > FileImage.maxFileAlign)
                verInfo.fileAlign = FileImage.maxFileAlign;
            text.SetSize(NumToAlign(text.Tide(), verInfo.fileAlign));
            if(Diag.DiagOn) {
                Console.WriteLine("text size = " + text.Size() + " text tide = " + text.Tide() + " text padding = " + text.Padding());
                Console.WriteLine("metaDataOffset = " + Hex.Int(metaDataOffset));
                Console.WriteLine("importTableOffset = " + Hex.Int(importTableOffset));
                Console.WriteLine("importLookupTableOffset = " + Hex.Int(importLookupTableOffset));
                Console.WriteLine("hintNameTableOffset = " + Hex.Int(hintNameTableOffset));
                Console.WriteLine("runtimeEngineOffset = " + Hex.Int(runtimeEngineOffset));
                Console.WriteLine("entryPointOffset = " + Hex.Int(entryPointOffset));
                Console.WriteLine("entryPointPadding = " + Hex.Int(entryPointPadding));
            }
        }

        internal void BuildRelocSection()
        {
            // do entry point reloc
            uint relocPage = entryPointReloc / Section.relocPageSize;
            uint pageOff = relocPage * Section.relocPageSize;
            reloc.Write(text.RVA() + pageOff);
            reloc.Write(12);
            uint fixUpOff = entryPointReloc - pageOff;
            reloc.Write((ushort)((0x3 << 12) | fixUpOff));
            reloc.Write((ushort)0);
            // text.DoRelocs(reloc);
            if(sdata != null)
                sdata.DoRelocs(reloc);
            if(rsrc != null)
                rsrc.DoRelocs(reloc);
            relocTide = (uint)reloc.Seek(0, SeekOrigin.Current);
            //reloc.Write((uint)0);
            if(Diag.DiagOn)
                Console.WriteLine("relocTide = " + relocTide);
            relocPadding = NumToAlign(relocTide, verInfo.fileAlign);
            relocSize = relocTide + relocPadding;
            imageSize = relocRVA + FileImage.SectionAlignment;
            initDataSize += relocSize;
        }

        private void CalcOffsets()
        {
            if(sdata != null)
                numSections++;
            if(rsrc != null)
                numSections++;
            headerSize = FileImage.fileHeaderSize + (numSections * FileImage.sectionHeaderSize);
            headerPadding = NumToAlign(headerSize, verInfo.fileAlign);
            headerSize += headerPadding;
            uint offset = headerSize;
            uint rva = FileImage.SectionAlignment;
            text.SetOffset(offset);
            text.SetRVA(rva);
            //if (pdbWriter != null) debugRVA = rva + debugOffset; 
            offset += text.Size();
            rva  = GetNextSectStart(rva, text.Tide());
            // Console.WriteLine("headerSize = " + headerSize);
            // Console.WriteLine("headerPadding = " + headerPadding);
            // Console.WriteLine("textOffset = " + Hex.Int(text.Offset()));
            if(sdata != null) {
                sdata.SetOffset(offset);
                sdata.SetRVA(rva);
                for(int i = 0; i < data.Count; i++) {
                    DataConstant cVal = (DataConstant)data[i];
                    cVal.DataOffset = sdata.Tide();
                    sdata.IncTide(cVal.GetSize());
                }
                sdata.SetSize(NumToAlign(sdata.Tide(), verInfo.fileAlign));
                offset += sdata.Size();
                rva = GetNextSectStart(rva, sdata.Tide());
                initDataSize += sdata.Size();
            }
            if(rsrc != null) {
                //Console.WriteLine("Resource section is not null");
                rsrc.SetSize(NumToAlign(rsrc.Tide(), verInfo.fileAlign));
                rsrc.SetOffset(offset);
                rsrc.SetRVA(rva);
                offset += rsrc.Size();
                rva = GetNextSectStart(rva, rsrc.Tide());
                initDataSize += rsrc.Size();
            }
            relocOffset = offset;
            relocRVA = rva;
        }

        /*   internal void SetPDBFile(string pdbFileName) {
             if (pdbFileName != null) {
               pdbWriter = PERWAPI.PDB.Impl.ObtainWriter(this, pdbFile);
               debugBytes = pdbWriter.GetDebugInfo();
             }
           }
           */

        internal void MakeFile(PEFileVersionInfo verInfo)
        {
            this.verInfo = verInfo;
            if(this.verInfo.isDLL)
                hintNameTable = FileImage.dllHintNameTable.ToCharArray();
            else
                hintNameTable = FileImage.exeHintNameTable.ToCharArray();
            BuildTextSection();
            CalcOffsets();
            BuildRelocSection();
            // now write it out
            WriteHeader();
            WriteSections();
            Flush();
            if(closeStream)
                Close();
            //if (pdbWriter != null) pdbWriter.WritePDBFile();
        }

        private void WriteHeader()
        {
            Write(FileImage.DOSHeader);
            // Console.WriteLine("Writing PEHeader at offset " + Seek(0,SeekOrigin.Current));
            WritePEHeader();
            // Console.WriteLine("Writing text section header at offset " + Hex.Long(Seek(0,SeekOrigin.Current)));
            text.WriteHeader(this, relocRVA);
            if(sdata != null)
                sdata.WriteHeader(this, relocRVA);
            if(rsrc != null)
                rsrc.WriteHeader(this, relocRVA);
            // Console.WriteLine("Writing reloc section header at offset " + Seek(0,SeekOrigin.Current));
            WriteRelocSectionHeader();
            // Console.WriteLine("Writing padding at offset " + Seek(0,SeekOrigin.Current));
            WriteZeros(headerPadding);
        }

        private void WriteSections()
        {
            // Console.WriteLine("Writing text section at offset " + Seek(0,SeekOrigin.Current));
            WriteTextSection();
            if(sdata != null)
                WriteSDataSection();
            if(rsrc != null)
                WriteRsrcSection();
            WriteRelocSection();
        }

        private void WriteIAT()
        {
            Write(text.RVA() + hintNameTableOffset);
            Write(0);
        }

        private void WriteImportTables()
        {
            // Import Table
            WriteZeros(importTablePadding);
            // Console.WriteLine("Writing import tables at offset " + Hex.Long(Seek(0,SeekOrigin.Current)));
            Write(importLookupTableOffset + text.RVA());
            WriteZeros(8);
            Write(runtimeEngineOffset + text.RVA());
            Write(text.RVA());    // IAT is at the beginning of the text section
            WriteZeros(20);
            // Import Lookup Table
            WriteIAT();                // lookup table and IAT are the same
            // Hint/Name Table
            // Console.WriteLine("Writing hintname table at " + Hex.Long(Seek(0,SeekOrigin.Current)));
            Write(hintNameTable);
            Write(FileImage.runtimeEngineName.ToCharArray());
        }

        private void WriteTextSection()
        {
            WriteIAT();
            WriteCLIHeader();
            if(Diag.DiagOn)
                Console.WriteLine("Writing code at " + Hex.Long(Seek(0, SeekOrigin.Current)));
            metaData.WriteByteCodes(this);
            if(Diag.DiagOn)
                Console.WriteLine("Finished writing code at " + Hex.Long(Seek(0, SeekOrigin.Current)));
            //largeStrings = metaData.LargeStringsIndex();
            //largeGUID = metaData.LargeGUIDIndex();
            //largeUS = metaData.LargeUSIndex();
            //largeBlob = metaData.LargeBlobIndex();
            //WriteDebugInfo();
            metaData.WriteMetaData(this);
            metaData.WriteResources(this);
            WriteImportTables();
            WriteZeros(entryPointPadding);
            Write((ushort)0x25FF);
            Write(FileImage.ImageBase + text.RVA());
            WriteZeros(text.Padding());
        }

        /*    private void WriteDebugInfo() {
         *    if (pdbWriter != null) {                // WINNT.h IMAGE_DEBUG_DIRECTORY
         *      WriteZeros(4);                        // Characteristics
         *      Write(dateStamp);                     // Date stamp
         *      WriteZeros(4);                        // Major Version, Minor Version
         *      Write(2);                             // Type  (Code View???)
         *      Write(debugBytes.Length);             // Size of Data
         *      WriteZeros(4);                        // Address of Raw Data
         *      Write(text.Offset() + debugOffset + debugSize);     // Pointer to Raw Data
         *      Write(debugBytes);
         *      WriteZeros(NumToAlign((uint)debugBytes.Length,4));
         *    }
         *  }
         */

        private void WriteCLIHeader()
        {
            Write(FileImage.CLIHeaderSize);       // Cb
            Write(verInfo.cliMajVer);            // Major runtime version
            Write(verInfo.cliMinVer);            // Minor runtime version
            Write(text.RVA() + metaDataOffset);
            if(Diag.DiagOn)
                Console.WriteLine("MetaDataOffset = " + metaDataOffset);
            Write(metaData.Size());
            Write((uint)verInfo.corFlags);
            Write(entryPointToken);
            if(resourcesSize > 0) {  // managed resources
                Write(text.RVA() + resourcesOffset);
                Write(resourcesSize);
            } else {
                WriteZeros(8);
            }
            WriteZeros(8);                     // Strong Name stuff here!! NYI
            WriteZeros(8);                     // CodeManagerTable
            WriteZeros(8);                     // VTableFixups NYI
            WriteZeros(16);                    // ExportAddressTableJumps, ManagedNativeHeader
        }

        private void WriteSDataSection()
        {
            long pos = BaseStream.Position;
            for(int i=0; i < data.Count; i++) {
                ((DataConstant)data[i]).Write(this);
            }
            pos = BaseStream.Position - pos;
            WriteZeros(NumToAlign((uint)pos, verInfo.fileAlign));
        }

        private void WriteRsrcSection()
        {
            Console.WriteLine("Trying to write rsrc section !!!");
        }

        private void WriteRelocSection()
        {
            // Console.WriteLine("Writing reloc section at " + Seek(0,SeekOrigin.Current) + " = " + relocOffset);
            MemoryStream str = (MemoryStream)reloc.BaseStream;
            Write(str.ToArray());
            WriteZeros(NumToAlign((uint)str.Position, verInfo.fileAlign));
        }

        internal void SetEntryPoint(uint entryPoint)
        {
            entryPointToken = entryPoint;
        }

        internal void AddInitData(DataConstant cVal)
        {
            if(sdata == null) {
                sdata = new Section(FileImage.sdataName, 0xC0000040);   // IMAGE_SCN_CNT  INITIALIZED_DATA, READ, WRITE
                data = new ArrayList();
            }
            data.Add(cVal);
            //cVal.DataOffset = sdata.Tide();
            //sdata.IncTide(cVal.GetSize());
        }

        internal void WriteZeros(uint numZeros)
        {
            for(int i=0; i < numZeros; i++) {
                Write((byte)0);
            }
        }

        internal void WritePEHeader()
        {
            Write((ushort)0x014C);  // Machine - always 0x14C for Managed PE Files (allow others??)
            Write((ushort)numSections);
            Write(dateStamp);
            WriteZeros(8); // Pointer to Symbol Table and Number of Symbols (always zero for ECMA CLI files)
            Write((ushort)0x00E0);  // Size of Optional Header
            Write(verInfo.characteristics);
            // PE Optional Header
            Write((ushort)0x010B);   // Magic
            Write(verInfo.lMajor);        // LMajor pure-IL = 6   C++ = 7
            Write(verInfo.lMinor);
            Write(text.Size());
            Write(initDataSize);
            Write(0);                // Check other sections here!!
            Write(text.RVA() + entryPointOffset);
            Write(text.RVA());
            uint dataBase = 0;
            if(sdata != null)
                dataBase = sdata.RVA();
            else if(rsrc != null)
                dataBase = rsrc.RVA();
            else
                dataBase = relocRVA;
            Write(dataBase);
            Write(FileImage.ImageBase);
            Write(FileImage.SectionAlignment);
            Write(verInfo.fileAlign);
            Write(verInfo.osMajor);
            Write(verInfo.osMinor);
            Write(verInfo.userMajor);
            Write(verInfo.userMinor);
            Write(verInfo.subSysMajor);     // OS Major
            Write(verInfo.subSysMinor);
            WriteZeros(4);           // Reserved
            Write(imageSize);
            Write(headerSize);
            Write((int)0);           // File Checksum
            Write((ushort)verInfo.subSystem);
            Write(verInfo.DLLFlags);
            Write(FileImage.StackReserveSize);
            Write(FileImage.StackCommitSize);
            Write(FileImage.HeapReserveSize);
            Write(FileImage.HeapCommitSize);
            Write(FileImage.LoaderFlags);
            Write(FileImage.NumDataDirectories);  // Data Directories
            WriteZeros(8);                  // Export Table
            Write(importTableOffset + text.RVA());
            Write(totalImportTableSize);
            WriteZeros(24);            // Resource, Exception and Certificate Tables
            Write(relocRVA);
            Write(relocTide);
            Write(debugRVA);
            Write(debugSize);
            WriteZeros(40);            // Copyright, Global Ptr, TLS, Load Config and Bound Import Tables
            Write(text.RVA());         // IATRVA - IAT is at start of .text Section
            Write(FileImage.IATSize);
            WriteZeros(8);             // Delay Import Descriptor
            Write(text.RVA()+FileImage.IATSize); // CLIHeader immediately follows IAT
            Write(FileImage.CLIHeaderSize);
            WriteZeros(8);             // Reserved
        }

        internal void WriteRelocSectionHeader()
        {
            Write(FileImage.relocName.ToCharArray());
            Write(relocTide);
            Write(relocRVA);
            Write(relocSize);
            Write(relocOffset);
            WriteZeros(12);
            Write(FileImage.relocFlags);
        }

        private void Align(MemoryStream str, int val)
        {
            if((str.Position % val) != 0) {
                for(int i=val - (int)(str.Position % val); i > 0; i--) {
                    str.WriteByte(0);
                }
            }
        }

        private uint Align(uint val, uint alignVal)
        {
            if((val % alignVal) != 0) {
                val += alignVal - (val % alignVal);
            }
            return val;
        }

        private uint NumToAlign(uint val, uint alignVal)
        {
            if((val % alignVal) == 0)
                return 0;
            return alignVal - (val % alignVal);
        }

        internal void StringsIndex(uint ix)
        {
            if(metaData.largeStrings)
                Write(ix);
            else
                Write((ushort)ix);
        }

        internal void GUIDIndex(uint ix)
        {
            if(metaData.largeGUID)
                Write(ix);
            else
                Write((ushort)ix);
        }

        internal void USIndex(uint ix)
        {
            if(metaData.largeUS)
                Write(ix);
            else
                Write((ushort)ix);
        }

        internal void BlobIndex(uint ix)
        {
            if(metaData.largeBlob)
                Write(ix);
            else
                Write((ushort)ix);
        }

        internal void WriteIndex(MDTable tabIx, uint ix)
        {
            if(metaData.LargeIx(tabIx))
                Write(ix);
            else
                Write((ushort)ix);
        }

        internal void WriteCodedIndex(CIx code, MetaDataElement elem)
        {
            metaData.WriteCodedIndex(code, elem, this);
        }

        internal void WriteCodeRVA(uint offs)
        {
            Write(text.RVA() + codeStart + offs);
        }

        internal void WriteDataRVA(uint offs)
        {
            Write(sdata.RVA() + offs);
        }

        internal void Write3Bytes(uint val)
        {
            byte b3 = (byte)((val & FileImage.iByteMask[2]) >> 16);
            byte b2 = (byte)((val & FileImage.iByteMask[1]) >> 8);
            ;
            byte b1 = (byte)(val & FileImage.iByteMask[0]);
            Write(b1);
            Write(b2);
            Write(b3);
        }

    }
    /**************************************************************************/
    // Class to Read PE Files
    /**************************************************************************/

    internal class MetaDataTables {
        private TableRow[][] tables;

        internal MetaDataTables(TableRow[][] tabs)
        {
            tables = tabs;
        }

        internal MetaDataElement GetTokenElement(uint token)
        {
            uint tabIx = (token & FileImage.TableMask) >> 24;
            uint elemIx = (token & FileImage.ElementMask) - 1;
            return (MetaDataElement)tables[tabIx][(int)elemIx];
        }

    }

    internal partial class PEReader : BinaryReader {

        internal static long blobStreamStartOffset = 0;
        private static readonly int cliIx = 14;
        private Section[] inputSections;
        int numSections = 0;
        uint[] DataDirectoryRVA = new uint[16];
        uint[] DataDirectorySize = new uint[16];
        uint[] streamOffsets, streamSizes;
        String[] streamNames;
        private TableRow[][] tables = new TableRow[MetaData.NumMetaDataTables][];
        uint[] tableLengths = new uint[MetaData.NumMetaDataTables];
        MetaData md = new MetaData();
        MetaDataStringStream userstring;
        MetaDataInStream blob, strings, guid;
        Sentinel sentinel = new Sentinel();
        Pinned pinned = new Pinned();
        //CorFlags corFlags;
        uint metaDataRVA = 0, metaDataSize = 0, flags = 0;
        uint entryPoint = 0, resourcesRVA = 0, resourcesSize = 0;
        uint strongNameRVA = 0, strongNameSize = 0, vFixupsRVA = 0, vFixupsSize = 0;
        //ushort dllFlags = 0, subSystem = 0;
        //uint fileAlign = 0;
        //char[] verString;
        bool refsOnly = false;
        long[] tableStarts;
        ResolutionScope thisScope;
        PEFileVersionInfo verInfo = new PEFileVersionInfo();
        internal Method currentMethodScope;
        internal Class currentClassScope;
        int genInstNestLevel = 0;

        private static System.IO.FileStream GetFile(string filename)
        {
            if(Diag.DiagOn) {
                Console.WriteLine("Current directory is " + System.Environment.CurrentDirectory);
                Console.WriteLine("Looking for file " + filename);
            }
            if(System.IO.File.Exists(filename)) {
                return System.IO.File.OpenRead(filename);
            } else
                throw (new System.IO.FileNotFoundException("File Not Found", filename));
        }

        internal static ReferenceScope GetExportedInterface(string filename)
        {
            System.IO.FileStream file = GetFile(filename);
            PEReader reader = new PEReader(null, file, true);
            return (ReferenceScope)reader.thisScope;
        }

        //internal ResolutionScope GetThisScope() { return thisScope; }

        internal string[] GetAssemblyRefNames()
        {
            string[] assemNames = new string[tableLengths[(int)MDTable.AssemblyRef]];
            for(int i=0; i < assemNames.Length; i++) {
                assemNames[i] = ((AssemblyRef)tables[(int)MDTable.AssemblyRef][i]).Name();
            }
            return assemNames;
        }

        internal AssemblyRef[] GetAssemblyRefs()
        {
            AssemblyRef[] assemRefs = new AssemblyRef[tableLengths[(int)MDTable.AssemblyRef]];
            for(int i=0; i < assemRefs.Length; i++) {
                assemRefs[i] = (AssemblyRef)tables[(int)MDTable.AssemblyRef][i];
            }
            return assemRefs;
        }
        /*----------------------------- Reading ----------------------------------------*/

        internal void InputError()
        {
            throw new PEFileException("Error in input");
        }

        internal void MetaDataError(string msg)
        {
            msg = "ERROR IN METADATA: " + msg;
            if(thisScope != null)
                msg = "MODULE " + thisScope.Name() + ": " + msg;
            throw new PEFileException(msg);
        }

        internal Section GetSection(uint rva)
        {
            for(int i=0; i < inputSections.Length; i++) {
                if(inputSections[i].ContainsRVA(rva))
                    return inputSections[i];
            }
            return null;
        }

        internal uint GetOffset(uint rva)
        {
            for(int i=0; i < inputSections.Length; i++) {
                if(inputSections[i].ContainsRVA(rva))
                    return inputSections[i].GetOffset(rva);
            }
            return 0;
        }

        internal void ReadZeros(int num)
        {
            for(int i=0; i < num; i++) {
                byte next = ReadByte();
                if(next != 0)
                    InputError();
            }
        }

        private void ReadDOSHeader()
        {
            for(int i=0; i < FileImage.PESigOffset; i++) {
                if(FileImage.DOSHeader[i] != ReadByte()) {
                    InputError();
                }
            }
            uint sigOffset = ReadUInt32();
            for(int i = FileImage.PESigOffset+4; i < FileImage.DOSHeader.Length; i++) {
                if(FileImage.DOSHeader[i] != ReadByte()) {
                    InputError();
                }
            }
            BaseStream.Seek(sigOffset, SeekOrigin.Begin);
            if((char)ReadByte() != 'P')
                InputError();
            if((char)ReadByte() != 'E')
                InputError();
            if(ReadByte() != 0)
                InputError();
            if(ReadByte() != 0)
                InputError();
        }

        private void ReadFileHeader()
        {
            // already read PE signature
            if(ReadUInt16() != FileImage.machine)
                InputError();
            numSections = ReadUInt16();
            uint TimeStamp = ReadUInt32();
            ReadZeros(8);     /* Pointer to Symbol Table, Number of Symbols */
            int optHeadSize = ReadUInt16();
            verInfo.characteristics = ReadUInt16();
            verInfo.isDLL = (verInfo.characteristics & FileImage.dllFlag) != 0;
            /* Now read PE Optional Header */
            /* Standard Fields */
            if(ReadUInt16() != FileImage.magic)
                InputError();
            verInfo.lMajor = ReadByte(); // != FileImage.lMajor) InputError();
            verInfo.lMinor = ReadByte(); // != FileImage.lMinor) InputError();
            uint codeSize = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("codeSize = " + Hex.Int(codeSize));
            uint initDataSize = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("initDataSize = " + Hex.Int(initDataSize));
            uint uninitDataSize = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("uninitDataSize = " + Hex.Int(uninitDataSize));
            uint entryPointRVA = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("entryPointRVA = " + Hex.Int(entryPointRVA));
            uint baseOfCode = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("baseOfCode = " + Hex.Int(baseOfCode));
            uint baseOfData = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("baseOfData = " + Hex.Int(baseOfData));
            /* NT-Specific Fields */
            uint imageBase = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("imageBase = " + Hex.Int(imageBase));
            uint sectionAlign = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("sectionAlign = " + Hex.Int(sectionAlign));
            verInfo.fileAlign = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("fileAlign = " + Hex.Int(verInfo.fileAlign));
            verInfo.osMajor = ReadUInt16();
            if(Diag.DiagOn)
                Console.WriteLine("osMajor = " + Hex.Int(verInfo.osMajor));
            //ReadZeros(6);         // osMinor, userMajor, userMinor
            verInfo.osMinor = ReadUInt16();
            verInfo.userMajor = ReadUInt16();
            verInfo.userMinor = ReadUInt16();
            verInfo.subSysMajor = ReadUInt16();
            if(Diag.DiagOn)
                Console.WriteLine("subsysMajor = " + Hex.Int(verInfo.subSysMajor));
            verInfo.subSysMinor = ReadUInt16();
            ReadZeros(4);         // Reserved
            uint imageSize = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("imageSize = " + Hex.Int(imageSize));
            uint headerSize = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("headerSize = " + Hex.Int(headerSize));
            uint checkSum = ReadUInt32();
            if(Diag.DiagOn)
                Console.WriteLine("checkSum = " + Hex.Int(checkSum));
            verInfo.subSystem = (SubSystem)ReadUInt16();
            if(Diag.DiagOn)
                Console.WriteLine("subSystem = " + Hex.Short((int)verInfo.subSystem));
            verInfo.DLLFlags = ReadUInt16();
            if(Diag.DiagOn)
                Console.WriteLine("DLLFlags = " + Hex.Short(verInfo.DLLFlags));
            if(ReadUInt32() != FileImage.StackReserveSize)
                InputError();
            if(ReadUInt32() != FileImage.StackCommitSize)
                InputError();
            if(ReadUInt32() != FileImage.HeapReserveSize)
                InputError();
            if(ReadUInt32() != FileImage.HeapCommitSize)
                InputError();
            if(ReadUInt32() != 0)
                InputError(); // LoaderFlags
            if(ReadUInt32() != FileImage.NumDataDirectories)
                InputError();
            /* Data Directories */
            DataDirectoryRVA = new uint[FileImage.NumDataDirectories];
            DataDirectorySize = new uint[FileImage.NumDataDirectories];
            for(int i = 0; i < FileImage.NumDataDirectories; i++) {
                DataDirectoryRVA[i] = ReadUInt32();
                DataDirectorySize[i] = ReadUInt32();
            }
            if(Diag.DiagOn) {
                Console.WriteLine("RVA = " + Hex.Int(DataDirectoryRVA[1]) + "  Size = " + Hex.Int(DataDirectorySize[1]) + "  Import Table");
                Console.WriteLine("RVA = " + Hex.Int(DataDirectoryRVA[2]) + "  Size = " + Hex.Int(DataDirectorySize[2]) + "  Resource Table");
                Console.WriteLine("RVA = " + Hex.Int(DataDirectoryRVA[5]) + "  Size = " + Hex.Int(DataDirectorySize[5]) + "  Base Relocation Table");
                Console.WriteLine("RVA = " + Hex.Int(DataDirectoryRVA[12]) + "  Size = " + Hex.Int(DataDirectorySize[12]) + "  IAT");
                Console.WriteLine("RVA = " + Hex.Int(DataDirectoryRVA[14]) + "  Size = " + Hex.Int(DataDirectorySize[14]) + "  CLI Header");
            }
        }

        private void ReadSectionHeaders()
        {
            if(Diag.DiagOn)
                Console.WriteLine("Sections");
            inputSections = new Section[numSections];
            for(int i=0; i < numSections; i++) {
                inputSections[i] = new Section(this);
            }
        }

        private void ReadCLIHeader()
        {
            BaseStream.Seek(GetOffset(DataDirectoryRVA[cliIx]), SeekOrigin.Begin);
            uint cliSize = ReadUInt32();
            verInfo.cliMajVer = ReadUInt16(); // check
            verInfo.cliMinVer = ReadUInt16(); // check
            metaDataRVA = ReadUInt32();
            metaDataSize = ReadUInt32();
            //Console.WriteLine("Meta Data at rva " + PEConsts.Hex(metaDataRVA) + "  size = " + PEConsts.Hex(metaDataSize));
            verInfo.corFlags = (CorFlags)ReadUInt32();
            entryPoint = ReadUInt32();
            resourcesRVA = ReadUInt32();
            resourcesSize = ReadUInt32();
            strongNameRVA = ReadUInt32();
            strongNameSize = ReadUInt32();
            ReadZeros(8); // CodeManagerTable
            vFixupsRVA = ReadUInt32();
            vFixupsSize = ReadUInt32();
            ReadZeros(16); // ExportAddressTableJumps/ManagedNativeHeader
        }

        private String ReadStreamName()
        {
            char[] strName = new char[9];
            strName[0] = (char)ReadByte();
            char ch = (char)ReadByte();
            int i=1;
            while(ch != '\0') {
                strName[i++] = ch;
                ch = (char)ReadByte();
            }
            strName[i++] = '\0';
            if(i % 4 != 0) {
                for(int j = 4 - i % 4; j > 0; j--)
                    ReadByte();
            }
            return new String(strName, 0, i-1);
        }

        private void ReadMetaData()
        {
            if(Diag.DiagOn)
                Console.WriteLine("MetaData at RVA = " + Hex.Int(metaDataRVA) + " and offset = " + Hex.Int(GetOffset(metaDataRVA)));
            BaseStream.Seek(GetOffset(metaDataRVA), SeekOrigin.Begin);
            uint sig = ReadUInt32();              // check
            verInfo.mdMajVer = ReadUInt16();           // check
            verInfo.mdMinVer = ReadUInt16();           // check
            ReadZeros(4);
            int verStrLen = ReadInt32();
            int end = -1;
            char[] verString = new char[verStrLen+1];
            for(int i=0; i < verStrLen; i++) {
                verString[i] = (char)ReadByte();
                if((verString[i] == 0) && (end == -1))
                    end = i;
            }
            verString[verStrLen] = (char)0; // check
            if(end == -1)
                end = verStrLen;
            verInfo.netVerString = new string(verString, 0, end);
            GenericParam.extraField = verInfo.netVerString.CompareTo(MetaData.versions[1]) <= 0; // after version 2.0.40607
            if(Diag.DiagOn && GenericParam.extraField) {
                Console.WriteLine("Version = " + verInfo.netVerString + " has extra field for GenericParam");
            }
            int alignNum = 0;
            if((verStrLen % 4) != 0)
                alignNum = 4 - (verStrLen % 4);
            ReadZeros(alignNum);
            flags = ReadUInt16(); // check
            int numStreams = ReadUInt16();
            streamOffsets = new uint[numStreams];
            streamSizes = new uint[numStreams];
            streamNames = new String[numStreams];
            if(Diag.DiagOn)
                Console.WriteLine("MetaData Streams");
            for(int i=0; i < numStreams; i++) {
                streamOffsets[i] = ReadUInt32();
                streamSizes[i] = ReadUInt32();
                streamNames[i] = ReadStreamName();
                if(Diag.DiagOn)
                    Console.WriteLine("  " + streamNames[i] + "  Offset = " + Hex.Int(streamOffsets[i]) + "  Size = " + Hex.Int(streamSizes[i]));
            }
            uint tildeIx = 0;
            for(uint i=0; i < numStreams; i++) {
                String nam = streamNames[i];
                if(MetaData.tildeName.CompareTo(nam) == 0)
                    tildeIx = i;
                else {
                    uint streamoff = GetOffset(metaDataRVA + streamOffsets[i]);
                    if(Diag.DiagOn)
                        Console.WriteLine("getting stream bytes at offset " + Hex.Int(streamoff));
                    BaseStream.Seek(GetOffset(metaDataRVA+streamOffsets[i]), SeekOrigin.Begin);
                    long streamStart = BaseStream.Position;
                    byte[] strBytes = ReadBytes((int)streamSizes[i]);
                    if(MetaData.stringsName.CompareTo(nam) == 0) {
                        strings = new MetaDataInStream(strBytes);
                    } else if(MetaData.userstringName.CompareTo(nam) == 0) {
                        userstring = new MetaDataStringStream(strBytes);
                    } else if(MetaData.blobName.CompareTo(nam) == 0) {
                        blobStreamStartOffset = streamStart;
                        blob = new MetaDataInStream(strBytes);
                    } else if(MetaData.guidName.CompareTo(nam) == 0) {
                        guid = new MetaDataInStream(strBytes);
                    } else if(nam.CompareTo("#-") == 0) {
                        tildeIx = i;
                        //throw new Exception("Illegal uncompressed data stream #-");
                    } else {
                        Console.WriteLine("Unknown stream - " + nam);
                    }
                }
            }
            // go to beginning of tilde stream
            BaseStream.Seek(GetOffset(metaDataRVA+streamOffsets[tildeIx]), SeekOrigin.Begin);
            ReadTildeStreamStart();
        }

        private void SetUpTableInfo()
        {
            md.CalcElemSize();
            tableStarts = new long[MetaData.NumMetaDataTables];
            long currentPos = BaseStream.Position;
            for(int ix=0; ix < MetaData.NumMetaDataTables; ix++) {
                tableStarts[ix] = currentPos;
                currentPos += tableLengths[ix] * md.elemSize[ix];
            }
        }

        private void ReadTildeStreamStart()
        {
            if(Diag.DiagOn)
                Console.WriteLine("Reading meta data tables at offset = " + Hex.Int((int)BaseStream.Position));
            // pre:  at beginning of tilde stream
            ReadZeros(4);  // reserved
            verInfo.tsMajVer = ReadByte();  // check
            verInfo.tsMinVer = ReadByte();  // check
            byte heapSizes = ReadByte();
            if(heapSizes != 0) {
                md.largeStrings = (heapSizes & 0x01) != 0;
                md.largeGUID = (heapSizes & 0x02) != 0;
                md.largeBlob = (heapSizes & 0x04) != 0;
            }
            if(Diag.DiagOn) {
                if(md.largeStrings)
                    Console.WriteLine("LARGE strings index");
                if(md.largeGUID)
                    Console.WriteLine("LARGE GUID index");
                if(md.largeBlob)
                    Console.WriteLine("LARGE blob index");
            }
            int res = ReadByte(); // check if 1
            ulong valid = ReadUInt64();
            ulong sorted = this.ReadUInt64();
            if(Diag.DiagOn)
                Console.WriteLine("Valid = " + Hex.Long(valid));
            for(int i=0; i < MetaData.NumMetaDataTables; i++) {
                if((valid & FileImage.bitmasks[i]) != 0) {
                    tableLengths[i] = ReadUInt32();
                    tables[i] = new TableRow[tableLengths[i]];
                    md.largeIx[i] = tableLengths[i] > MetaData.maxSmlIxSize;
                    if(Diag.DiagOn)
                        Console.WriteLine("Table Ix " + Hex.Short(i) + " has length " + tableLengths[i]);
                } else
                    tableLengths[i] = 0;
            }
            if(tableLengths[0] != 1)
                this.MetaDataError("Module table has more than one entry");
            for(int i=0; i < MetaData.CIxTables.Length; i++) {
                for(int j=0; j < MetaData.CIxTables[i].Length; j++) {
                    if(Diag.DiagOn)
                        Console.WriteLine("CIxTables " + i + " " + j + " tableLength = " + tableLengths[MetaData.CIxTables[i][j]] + "  Max = " + MetaData.CIxMaxMap[i]);
                    md.lgeCIx[i] = md.lgeCIx[i] || 
            (tableLengths[MetaData.CIxTables[i][j]] > MetaData.CIxMaxMap[i]);
                }
                if(Diag.DiagOn)
                    if(md.lgeCIx[i])
                        Console.WriteLine("LARGE CIx " + i);
            }
        }

        private void SetThisScope()
        {
            if(refsOnly)
                thisScope = Module.ReadModuleRef(this);
            else
                ((PEFile)thisScope).Read(this);
            tables[(int)MDTable.Module][0] = thisScope;
            if(tableLengths[(int)MDTable.Assembly] > 0) {
                SetElementPosition(MDTable.Assembly, 1);
                if(refsOnly) {
                    ModuleRef thisMod = (ModuleRef)thisScope;
                    thisScope = Assembly.ReadAssemblyRef(this);
                    //if ((thisMod != null) && (thisMod.ismscorlib) && (thisScope != null)) {
                    //  ((AssemblyRef)thisScope).CopyVersionInfoToMSCorLib();
                    //  thisScope = MSCorLib.mscorlib;
                    //}
                    tables[(int)MDTable.Assembly][0] = thisScope;
                } else {
                    Assembly.Read(this, tables[(int)MDTable.Assembly], (PEFile)thisScope);
                    ((PEFile)thisScope).SetThisAssembly((Assembly)tables[(int)MDTable.Assembly][0]);
                }
            }
        }

        /// <summary>
        /// Read the Module metadata for this PE file.
        /// If reading refs only, then thisModule is the ModuleRef 
        /// If reading defs then pefile is the Module 
        /// </summary>
        /*    private void GetThisPEFileScope() {
              if (refsOnly) {
                thisModuleRef = Module.ReadModuleRef(this);
              } else { 
                pefile.Read(this);
                tables[(int)MDTable.Module][0] = pefile;
              }
            }

            private AssemblyRef GetThisAssembly(bool atPos) {
              if (tableLengths[(int)MDTable.Assembly] == 0) return null;
              if (!atPos)
                BaseStream.Position = tableStarts[(int)MDTable.Assembly];
              if (refsOnly) 
                tables[(int)MDTable.Assembly][0] = Assembly.ReadAssemblyRef(this);
              else 
                Assembly.Read(this,tables[(int)MDTable.Assembly],pefile);
              return (AssemblyRef)tables[(int)MDTable.Assembly][0];
            }
        */
        /*
        private ReferenceScope ReadRefsOnDemand() {
          ModuleRef thisModule;
          SetUpTableInfo();
          ResolutionScope mod;

          AssemblyRef thisAssembly = GetThisAssemblyRef();
          SetElementPosition(MDTable.Module,0);
          ReadZeros(2);
          name = buff.GetString();
          mvid = buff.GetGUID();
          ModuleRef thisMod = ModuleRef.GetModuleRef(name);
          if (thisMod == null) {
            thisMod = new ModuleRef(name);
            Module.AddToList(thisMod);
          } else {
          }
          if (thisModule == null) {
            thisModule = new ModuleRef(name);
            thisModule.readAsDef = true;
            if (mod != null) ((Module)mod).refOf = thisModule;
            else Module.AddToList(thisModule);
          } else {
            if (thisModule.readAsDef) return thisModule;
            return Merge(thisModule);
          }
          ReferenceScope thisScope = thisAssembly;
          if (thisScope == null) thisScope = thisModule;
          ClassRef defClass = ReadDefaultClass();
          thisScope.SetDefaultClass(defClass);
          ClassDef.GetClassRefNames(this,thisScope);
      
          return null;
        }
        */

        private void ReadMetaDataTableRefs()
        {
            SetUpTableInfo();
            SetThisScope();
            // ReadAssemblyRefs
            SetElementPosition(MDTable.AssemblyRef, 1);
            if(tableLengths[(int)MDTable.AssemblyRef] > 0)
                AssemblyRef.Read(this, tables[(int)MDTable.AssemblyRef]);
            // Read File Table (for ModuleRefs)
            //SetElementPosition(MDTable.File,1);
            if(tableLengths[(int)MDTable.File] > 0)
                FileRef.Read(this, tables[(int)MDTable.File]);
            // Read Exported Classes
            //SetElementPosition(MDTable.ExportedType,1);
            if(tableLengths[(int)MDTable.ExportedType] > 0)
                ExternClass.GetClassRefs(this, tables[(int)MDTable.ExportedType]);
            // Read ModuleRefs
            if(tableLengths[(int)MDTable.ModuleRef] > 0) {
                BaseStream.Position = tableStarts[(int)MDTable.ModuleRef];
                ModuleRef.Read(this, tables[(int)MDTable.ModuleRef], true);
            }
            uint[] parIxs = new uint[tableLengths[(int)MDTable.TypeDef]];
            BaseStream.Position = tableStarts[(int)MDTable.NestedClass];
            MapElem.ReadNestedClassInfo(this, tableLengths[(int)MDTable.NestedClass], parIxs);
            BaseStream.Position = tableStarts[(int)MDTable.TypeRef];
            // Read ClassRefs
            if(tableLengths[(int)MDTable.TypeRef] > 0)
                ClassRef.Read(this, tables[(int)MDTable.TypeRef], true);
            // Read ClassDefs and fields and methods
            ClassDef.GetClassRefs(this, tables[(int)MDTable.TypeDef], (ReferenceScope)thisScope, parIxs);
            for(int i=0; i < tableLengths[(int)MDTable.ExportedType]; i++) {
                ((ClassRef)tables[(int)MDTable.ExportedType][i]).ResolveParent(this, true);
            }
        }

        internal void SetElementPosition(MDTable tabIx, uint ix)
        {
            BaseStream.Position = tableStarts[(int)tabIx] + (md.elemSize[(int)tabIx] * (ix-1));
        }

        internal void ReadMethodImpls(ClassDef theClass, uint classIx)
        {
            SetElementPosition(MDTable.InterfaceImpl, 0);
            for(int i=0; (i < tableLengths[(int)MDTable.MethodImpl]); i++) {
                uint clIx = GetIndex(MDTable.TypeDef);
                uint bodIx = GetCodedIndex(CIx.MethodDefOrRef);
                uint declIx = GetCodedIndex(CIx.MethodDefOrRef);
                if(clIx == classIx) {
                    MethodImpl mImpl = new MethodImpl(this, theClass, bodIx, declIx);
                    theClass.AddMethodImpl(mImpl);
                    tables[(int)MDTable.MethodImpl][i] = mImpl;
                }
            }
        }

        internal void InsertInTable(MDTable tabIx, uint ix, MetaDataElement elem)
        {
            tables[(int)tabIx][ix-1] = elem;
        }

        private void CheckForRefMerges()
        {
            if(tableLengths[(int)MDTable.TypeRef] > 0) {
                for(int i=0; i < tableLengths[(int)MDTable.TypeRef]; i++) {
                    ((ClassRef)tables[(int)MDTable.TypeRef][i]).ResolveParent(this, false);
                }
            }
            if(tableLengths[(int)MDTable.MemberRef] > 0) {
                for(int i=0; i < tableLengths[(int)MDTable.MemberRef]; i++) {
                    Member memb = (Member)tables[(int)MDTable.MemberRef][i];
                    tables[(int)MDTable.MemberRef][i] = memb.ResolveParent(this);
                }
            }
        }

        internal void ReplaceSig(Signature sig, Type sigType)
        {
            tables[(int)MDTable.StandAloneSig][sig.Row-1] = sigType;
        }

        internal void GetGenericParams(MethodDef meth)
        {
            if(tables[(int)MDTable.GenericParam] != null) {
                for(int j=0; j < tables[(int)MDTable.GenericParam].Length; j++) {
                    ((GenericParam)tables[(int)MDTable.GenericParam][j]).CheckParent(meth, this);
                }
            }
        }

        private void ReadMetaDataTables()
        {
            ((PEFile)thisScope).Read(this);
            tables[(int)MDTable.Module][0] = thisScope;
            for(int ix=1; ix < MetaData.NumMetaDataTables; ix++) {
                if(tableLengths[ix] > 0) {
                    switch(ix) {
                    case ((int)MDTable.Assembly):
                        Assembly.Read(this, tables[ix], (PEFile)thisScope);
                        break;
                    case ((int)MDTable.AssemblyOS):
                    case ((int)MDTable.AssemblyProcessor):
                    case ((int)MDTable.AssemblyRefOS):
                    case ((int)MDTable.AssemblyRefProcessor):
                        // ignore
                        Console.WriteLine("Got uncompressed table " + (MDTable)ix);
                        BaseStream.Seek(tableLengths[ix]*md.elemSize[ix], SeekOrigin.Current);
                        break;
                    case ((int)MDTable.AssemblyRef):
                        AssemblyRef.Read(this, tables[ix]);
                        break;
                    //case 0x25 : AssemblyRefOS.Read(this,tables[ix]); break;
                    //case 0x24 : AssemblyRefProcessor.Read(this,tables[ix]); break;
                    case ((int)MDTable.ClassLayout):
                        ClassLayout.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.Constant):
                        ConstantElem.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.CustomAttribute):
                        CustomAttribute.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.DeclSecurity):
                        DeclSecurity.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.Event):
                        Event.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.EventMap):
                        MapElem.Read(this, tables[ix], MDTable.EventMap);
                        break;
                    case ((int)MDTable.ExportedType):
                        ExternClass.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.Field):
                        FieldDef.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.FieldLayout):
                        FieldLayout.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.FieldMarshal):
                        FieldMarshal.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.FieldRVA):
                        FieldRVA.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.File):
                        FileRef.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.GenericParam):
                        GenericParam.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.GenericParamConstraint):
                        GenericParamConstraint.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.ImplMap):
                        ImplMap.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.InterfaceImpl):
                        InterfaceImpl.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.ManifestResource):
                        ManifestResource.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.MemberRef):
                        Member.ReadMember(this, tables[ix]);
                        break;
                    case ((int)MDTable.Method):
                        MethodDef.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.MethodImpl):
                        MethodImpl.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.MethodSemantics):
                        MethodSemantics.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.MethodSpec):
                        MethodSpec.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.ModuleRef):
                        ModuleRef.Read(this, tables[ix], false);
                        break;
                    case ((int)MDTable.NestedClass):
                        MapElem.Read(this, tables[ix], MDTable.NestedClass);
                        tables[ix] = null;
                        break;
                    case ((int)MDTable.Param):
                        Param.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.Property):
                        Property.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.PropertyMap):
                        MapElem.Read(this, tables[ix], MDTable.PropertyMap);
                        break;
                    case ((int)MDTable.StandAloneSig):
                        Signature.Read(this, tables[ix]);
                        break;
                    case ((int)MDTable.TypeDef):
                        ClassDef.Read(this, tables[ix], ((PEFile)thisScope).isMSCorLib());
                        break;
                    case ((int)MDTable.TypeRef):
                        ClassRef.Read(this, tables[ix], false);
                        break;
                    case ((int)MDTable.TypeSpec):
                        TypeSpec.Read(this, tables[ix]);
                        break;
                    default:
                        throw (new PEFileException("Unknown MetaData Table Type"));
                    }
                }
            }
            CheckForRefMerges();
            for(int ix=0; ix < MetaData.NumMetaDataTables; ix++) {
                if((tables[ix] != null) && (ix != (int)MDTable.TypeSpec) &&
          (ix != (int)MDTable.MethodSpec)) {  // resolve type/method specs when referenced
                    for(int j=0; j < tables[ix].Length; j++) {
                        //tables[ix][j].Row = (uint)j+1;
                        // KJG fix 2005:02:23
                        //   Everett ILASM leaves gaps in table[10][x] ... 
                        //   so protect with a null test.
                        //
                        // ((MetaDataElement)tables[ix][j]).Resolve(this); // old line ...
                        //
                        if(tables[ix][j] != null) {
                            ((MetaDataElement)tables[ix][j]).Resolve(this);
                        } else {
                            Console.WriteLine();
                        }
                    }
                }
            }
            if(tableLengths[(int)MDTable.Assembly] > 0)
                ((PEFile)thisScope).SetThisAssembly((Assembly)tables[(int)MDTable.Assembly][0]);
            ((PEFile)thisScope).SetDefaultClass((ClassDef)tables[(int)MDTable.TypeDef][0]);
            for(int j=1; j < tables[(int)MDTable.TypeDef].Length; j++) {
                ((PEFile)thisScope).AddToClassList((ClassDef)tables[(int)MDTable.TypeDef][j]);
            }
            if(tableLengths[(int)MDTable.ManifestResource] > 0) {
                for(int j=0; j < tables[(int)MDTable.ManifestResource].Length; j++) {
                    ((PEFile)thisScope).AddToResourceList((ManifestResource)tables[(int)MDTable.ManifestResource][j]);
                }
            }
            if(entryPoint != 0) {
                MetaDataElement ep = GetTokenElement(entryPoint);
                if(ep is MethodDef)
                    ((MethodDef)ep).DeclareEntryPoint();
                else
                    ((ModuleFile)ep).SetEntryPoint();
            }
        }

        internal uint GetIndex(MDTable tabIx)
        {
            if(md.largeIx[(int)tabIx])
                return ReadUInt32();
            return ReadUInt16();
        }

        internal uint GetCodedIndex(CIx codedIx)
        {
            if(md.lgeCIx[(int)codedIx])
                return ReadUInt32();
            return ReadUInt16();
        }

        internal uint GetTableSize(MDTable tabIx)
        {
            return (uint)tableLengths[(int)tabIx];
        }

        internal byte[] GetResource(uint offset)
        {
            BaseStream.Position = GetOffset(resourcesRVA)+offset;
            uint resSize = ReadUInt32();
            return ReadBytes((int)resSize);
        }

        internal MetaDataElement GetTokenElement(uint token)
        {
            uint tabIx = (token & FileImage.TableMask) >> 24;
            uint elemIx = (token & FileImage.ElementMask) - 1;
            MetaDataElement elem = (MetaDataElement)tables[tabIx][(int)elemIx];
            if((elem != null) && (elem.unresolved)) {
                elem.Resolve(this);
                elem = (MetaDataElement)tables[tabIx][(int)elemIx];
            }
            return elem;
        }

        internal MetaDataElement GetElement(MDTable tabIx, uint ix)
        {
            if(ix == 0)
                return null;
            MetaDataElement elem = (MetaDataElement)tables[(int)tabIx][(int)ix-1];
            if((elem != null) && (elem.unresolved)) {
                elem.Resolve(this);
                elem = (MetaDataElement)tables[(int)tabIx][(int)ix-1];
            }
            return elem;
        }

        internal MetaDataElement GetCodedElement(CIx code, uint ix)
        {
            uint mask = (uint)MetaData.CIxBitMasks[MetaData.CIxShiftMap[(uint)code]];
            int tabIx = MetaData.CIxTables[(int)code][(ix & mask)];
            ix >>= MetaData.CIxShiftMap[(uint)code];
            if(ix == 0)
                return null;
            MetaDataElement elem = (MetaDataElement)tables[tabIx][(int)ix-1];
            if((elem != null) && (elem.unresolved)) {
                elem.Resolve(this);
                elem = (MetaDataElement)tables[tabIx][(int)ix-1];
            }
            return elem;
        }

        internal uint MakeCodedIndex(CIx code, MDTable tab, uint ix)
        {
            ix <<= MetaData.CIxShiftMap[(uint)code];
            ix &= (uint)tab;
            return ix;
        }

        internal MDTable CodedTable(CIx code, uint ix)
        {
            uint mask = (uint)MetaData.CIxBitMasks[MetaData.CIxShiftMap[(uint)code]];
            return (MDTable)MetaData.CIxTables[(int)code][(ix & mask)];
        }

        internal uint CodedIndex(CIx code, uint ix)
        {
            ix >>= MetaData.CIxShiftMap[(uint)code];
            return ix;
        }

        internal byte[] GetBlob()
        {
            /* pre:  buffer is at correct position to read blob index */
            uint ix;
            if(md.largeBlob)
                ix = ReadUInt32();
            else
                ix = ReadUInt16();
            return blob.GetBlob(ix);
        }

        internal byte[] GetBlob(uint ix)
        {
            return blob.GetBlob(ix);
        }

        internal uint GetBlobIx()
        {
            /* pre:  buffer is at correct position to read blob index */
            //if (Diag.CADiag) Console.WriteLine("Getting blob index at " + BaseStream.Position);
            if(md.largeBlob)
                return ReadUInt32();
            return ReadUInt16();
        }

        internal byte FirstBlobByte(uint ix)
        {
            blob.GoToIndex(ix);
            uint blobSize = blob.ReadCompressedNum();
            return blob.ReadByte();
        }

        internal Constant GetBlobConst(int constType)
        {
            uint ix;
            if(md.largeBlob)
                ix = ReadUInt32();
            else
                ix = ReadUInt16();
            blob.GoToIndex(ix);
            uint blobSize = blob.ReadCompressedNum();
            if(constType == (int)ElementType.String)
                return new StringConst(blob.ReadBytes((int)blobSize));
            return ReadConst(constType, blob);
        }

        /*
        internal Constant ReadConstBlob(int constType, uint blobIx) {
          blob.GoToIndex(blobIx);
          Console.WriteLine("Reading constant blob at index " + blobIx );
          uint blobSize = blob.ReadCompressedNum();
          Console.WriteLine("Got constant blob size of " + blobSize);
          return ReadConst(constType);
        }
        */

        internal static Constant ReadConst(int constType, BinaryReader blob)
        {
            switch(constType) {
            case ((int)ElementType.Boolean):
                return new BoolConst(blob.ReadByte() != 0);
            case ((int)ElementType.Char):
                return new CharConst(blob.ReadChar());
            case ((int)ElementType.I1):
                return new IntConst(blob.ReadSByte());
            case ((int)ElementType.U1):
                return new UIntConst(blob.ReadByte());
            case ((int)ElementType.I2):
                return new IntConst(blob.ReadInt16());
            case ((int)ElementType.U2):
                return new UIntConst(blob.ReadUInt16());
            case ((int)ElementType.I4):
                return new IntConst(blob.ReadInt32());
            case ((int)ElementType.U4):
                return new UIntConst(blob.ReadUInt32());
            case ((int)ElementType.I8):
                return new IntConst(blob.ReadInt64());
            case ((int)ElementType.U8):
                return new UIntConst(blob.ReadUInt64());
            case ((int)ElementType.R4):
                return new FloatConst(blob.ReadSingle());
            case ((int)ElementType.R8):
                return new DoubleConst(blob.ReadDouble());
            case ((int)ElementType.ClassType):
                return new ClassTypeConst(blob.ReadString());  //GetBlobString()); 
            case ((int)ElementType.String):
                return new StringConst(blob.ReadString());  //GetBlobString()); 
            case ((int)ElementType.Class):
                return new ClassTypeConst(blob.ReadString());  //GetBlobString()); 
            //uint junk = blob.ReadUInt32();  // need to read name??
            //return new NullRefConst();
            case ((int)ElementType.ValueType):  // only const value type is enum??
                return new IntConst(blob.ReadInt32());

            default:
                return null;
            }
        }

        internal string GetBlobString()
        {
            uint ix;
            if(md.largeBlob)
                ix = ReadUInt32();
            else
                ix = ReadUInt16();
            return blob.GetBlobString(ix);
        }

        internal string GetString()
        {
            uint ix;
            if(md.largeStrings)
                ix = ReadUInt32();
            else
                ix = ReadUInt16();
            return strings.GetString(ix);
        }

        internal string GetString(uint ix)
        {
            return strings.GetString(ix);
        }

        internal uint GetStringIx()
        {
            if(md.largeStrings)
                return ReadUInt32();
            else
                return ReadUInt16();
        }

        internal uint GetGUIDIx()
        {
            /* pre:  buffer is at correct position to read GUID index */
            if(md.largeGUID)
                return ReadUInt32();
            return ReadUInt16();
        }

        public Guid GetGUID()
        {
            uint ix;
            if(md.largeGUID)
                ix = ReadUInt32();
            else
                ix = ReadUInt16();
            return new Guid(guid.GetBlob(((ix-1)*16), 16));
        }

        public string GetUserString()
        {
            uint ix;
            if(md.largeUS)
                ix = ReadUInt32();
            else
                ix = ReadUInt16();
            return userstring.GetUserString(ix);
        }

        internal bool IsFieldSig(uint blobIx)
        {
            blob.GoToIndex(blobIx);
            uint blobSize = blob.ReadCompressedNum();
            byte fldByte = blob.ReadByte();
            return fldByte == Field.FieldTag;
        }

        internal MethSig ReadMethSig(Method thisMeth, uint blobIx)
        {
            blob.GoToIndex(blobIx);
            uint blobSize = blob.ReadCompressedNum();
            return ReadMethSig(thisMeth, false);
        }

        internal MethSig ReadMethSig(Method thisMeth, string name, uint blobIx)
        {
            blob.GoToIndex(blobIx);
            uint blobSize = blob.ReadCompressedNum();
            MethSig mSig = ReadMethSig(thisMeth, false);
            mSig.name = name;
            return mSig;
        }

        private MethSig ReadMethSig(Method currMeth, bool firstByteRead)
        {
            //Class currClass = null;
            //if (currMeth != null) currClass = (Class)currMeth.GetParent();
            MethSig meth = new MethSig(null);
            if(!firstByteRead) {
                byte firstByte = blob.ReadByte();
                if(firstByte == Field.FieldTag)
                    return null;
                meth.callConv =(CallConv)firstByte;
            }
            if((meth.callConv & CallConv.Generic) != 0) {
                meth.numGenPars = blob.ReadCompressedNum();
                if(currMeth is MethodRef) {
                    ((MethodRef)currMeth).MakeGenericPars(meth.numGenPars);
                } //else if (currMeth is MethodDef) {
                //GetGenericParams((MethodDef)currMeth);
                //}
            }
            uint parCount = blob.ReadCompressedNum();
            if(Diag.DiagOn)
                Console.WriteLine("Method sig has " + parCount + " parameters");
            meth.retType = GetBlobType();//currClass,currMeth);
            if(meth.retType == null)
                System.Diagnostics.Debug.Assert(meth.retType != null);
            int optParStart = -1;
            ArrayList pTypes = new ArrayList();
            for(int i=0; i < parCount; i++) {
                Type pType = GetBlobType();//currClass,currMeth);
                if(pType == sentinel) {
                    optParStart = i;
                    pType = GetBlobType();//currClass,currMeth);
                }
                if(Diag.DiagOn)
                    if(pType == null)
                        Console.WriteLine("Param type is null");
                pTypes.Add(pType);
            }
            if(optParStart > -1) {
                meth.numPars = (uint)optParStart;
                meth.numOptPars = parCount - meth.numPars;
                meth.optParTypes = new Type[meth.numOptPars];
                for(int i=0; i < meth.numOptPars; i++) {
                    meth.optParTypes[i] = (Type)pTypes[i+optParStart];
                }
            } else
                meth.numPars = parCount;
            meth.parTypes = new Type[meth.numPars];
            for(int i=0; i < meth.numPars; i++) {
                meth.parTypes[i] = (Type)pTypes[i];
            }
            return meth;
        }

        internal Type[] ReadMethSpecSig(uint blobIx)
        { //ClassDef currClass, Method currMeth, uint blobIx) {
            blob.GoToIndex(blobIx);
            uint blobSize = blob.ReadCompressedNum();
            if(blob.ReadByte() != MethodSpec.GENERICINST)
                throw new Exception("Not a MethodSpec signature");
            return GetListOfType(); //currClass,currMeth);
        }

        internal Type GetFieldType(uint blobIx)
        {
            //Console.WriteLine("Getting field type");
            blob.GoToIndex(blobIx);
            uint blobSize = blob.ReadCompressedNum();
            byte fldByte = blob.ReadByte();
            if(fldByte != 0x6)
                throw new Exception("Expected field signature");
            //if ((currClass != null) && (currClass is ClassRef))
            //  currClass = null;
            return GetBlobType(); //currClass,null);
        }

        internal Type GetBlobType(uint blobIx)
        { //Class currClass, Method currMeth, uint blobIx) {
            blob.GoToIndex(blobIx);
            uint blobSize = blob.ReadCompressedNum();
            return GetBlobType(); //currClass,currMeth);
        }

        private Type[] GetListOfType()
        { //Class currClass, Method currMeth) {
            uint numPars = blob.ReadCompressedNum();
            Type[] gPars = new Type[numPars];
            for(int i=0; i < numPars; i++) {
                gPars[i] = GetBlobType(); //currClass,currMeth);
            }
            return gPars;
        }

        private Type GetBlobType()
        { //Class currClass, Method currMeth) {
            byte typeIx = blob.ReadByte();
            if(Diag.DiagOn)
                Console.WriteLine("Getting blob type " + (ElementType)typeIx);
            if(typeIx < PrimitiveType.primitives.Length)
                return PrimitiveType.primitives[typeIx];
            switch(typeIx) {
            case ((int)ElementType.Ptr):
                return new UnmanagedPointer(GetBlobType()); //currClass,currMeth)); 
            case ((int)ElementType.ByRef):
                return new ManagedPointer(GetBlobType()); //currClass,currMeth)); 
            case ((int)ElementType.ValueType):
                //Console.WriteLine("Reading value type");
                uint vcIx = blob.ReadCompressedNum();
                Class vClass = (Class)GetCodedElement(CIx.TypeDefOrRef, vcIx);
                vClass.MakeValueClass();
                return vClass;
            case ((int)ElementType.Class):
                return (Class)GetCodedElement(CIx.TypeDefOrRef, blob.ReadCompressedNum());
            case ((int)ElementType.Array):
                Type elemType = GetBlobType(); //currClass,currMeth);
                int rank = (int)blob.ReadCompressedNum();
                int numSizes = (int)blob.ReadCompressedNum();
                int[] sizes = null;
                if(numSizes > 0) {
                    sizes = new int[numSizes];
                    for(int i=0; i < numSizes; i++)
                        sizes[i] = (int)blob.ReadCompressedNum();
                }
                int numBounds = (int)blob.ReadCompressedNum();
                int[] loBounds = null, hiBounds = null;
                if((numBounds > 0) && (numSizes > 0)) {
                    loBounds = new int[numBounds];
                    hiBounds = new int[numBounds];
                    for(int i=0; i < numBounds; i++) {
                        loBounds[i] = (int)blob.ReadCompressedNum();
                        hiBounds[i] = loBounds[i] + sizes[i] - 1;
                    }
                }
                if(numSizes == 0)
                    return new BoundArray(elemType, rank);
                if(numBounds == 0)
                    return new BoundArray(elemType, rank, sizes);
                return new BoundArray(elemType, rank, loBounds, hiBounds);
            case ((int)ElementType.TypedByRef):
                return PrimitiveType.TypedRef;
            case ((int)ElementType.I):
                return PrimitiveType.IntPtr;
            case ((int)ElementType.U):
                return PrimitiveType.UIntPtr;
            case ((int)ElementType.FnPtr):
                MethSig mSig = ReadMethSig(null, false);
                return new MethPtrType(mSig);
            case ((int)ElementType.Object):
                return PrimitiveType.Object;
            case ((int)ElementType.SZArray):
                return new ZeroBasedArray(GetBlobType()); //currClass,currMeth));
            case ((int)ElementType.CmodReqd):
            case ((int)ElementType.CmodOpt):
                Class modType = (Class)GetCodedElement(CIx.TypeDefOrRef, blob.ReadCompressedNum());
                return new CustomModifiedType(GetBlobType(), (CustomModifier)typeIx, modType);
            case ((int)ElementType.Sentinel):
                return sentinel;
            case ((int)ElementType.Pinned):
                return pinned;
            case ((int)ElementType.GenericInst):
                Class instType = (Class)GetBlobType();
                Class scopeSave = currentClassScope;
                if(genInstNestLevel > 0) {
                    currentClassScope = instType;
                }
                genInstNestLevel++;
                ClassSpec newClassSpec = new ClassSpec(instType, GetListOfType());
                genInstNestLevel--;
                if(genInstNestLevel > 0) {
                    currentClassScope = scopeSave;
                }
                return newClassSpec;
            case ((int)ElementType.Var):
                if(currentClassScope == null) {
                    //Console.WriteLine("GenericParam with currClass == null");
                    return GenericParam.AnonClassPar(blob.ReadCompressedNum());
                    //throw new Exception("No current class set");
                }
                return currentClassScope.GetGenPar(blob.ReadCompressedNum());
            case ((int)ElementType.MVar):
                if(currentMethodScope == null) {
                    //Console.WriteLine("GenericParam with currMeth == null");
                    return GenericParam.AnonMethPar(blob.ReadCompressedNum());
                    //throw new Exception("No current method set");
                }
                return currentMethodScope.GetGenericParam((int)blob.ReadCompressedNum());
            default:
                break;
            }
            return null;
        }

        internal NativeType GetBlobNativeType(uint blobIx)
        {
            blob.GoToIndex(blobIx);
            uint blobSize = blob.ReadCompressedNum();
            return GetBlobNativeType();
        }

        internal NativeType GetBlobNativeType()
        {
            byte typeIx = blob.ReadByte();
            if(typeIx == (byte)NativeTypeIx.Array) {
                return new NativeArray(GetBlobNativeType(), blob.ReadCompressedNum(),
                  blob.ReadCompressedNum(), blob.ReadCompressedNum());
            } else
                return NativeType.GetNativeType(typeIx);
        }

        internal Local[] ReadLocalSig(uint sigIx)
        { //Class currClass, Method currMeth, uint sigIx) {
            blob.GoToIndex(sigIx);
            uint blobSize = blob.ReadCompressedNum();
            if(blob.ReadByte() != LocalSig.LocalSigByte)
                InputError();
            uint count = blob.ReadCompressedNum();
            Local[] locals = new Local[count];
            for(uint i=0; i < count; i++) {
                Type lType = GetBlobType(); //currClass,currMeth);
                bool pinnedLocal = lType == pinned;
                if(pinnedLocal)
                    lType = GetBlobType(); //currClass,currMeth);
                locals[i] = new Local("loc"+i, lType, pinnedLocal);
            }
            return locals;
        }

        internal void ReadPropertySig(uint sigIx, Property prop)
        {
            blob.GoToIndex(sigIx);
            uint blobSize = blob.ReadCompressedNum();
            if((blob.ReadByte() & Property.PropertyTag) != Property.PropertyTag)
                InputError();
            uint count = blob.ReadCompressedNum();
            Type[] pars = new Type[count];
            prop.SetPropertyType(GetBlobType()); //prop.GetParent(),null));
            for(int i=0; i < count; i++)
                pars[i] = GetBlobType(); //prop.GetParent(),null);
            prop.SetPropertyParams(pars);
        }

        internal DataConstant GetDataConstant(uint rva, Type constType)
        {
            BaseStream.Seek(GetOffset(rva), SeekOrigin.Begin);
            if(constType is PrimitiveType) {
                switch(constType.GetTypeIndex()) {
                case ((int)ElementType.I1):
                    return new IntConst(ReadByte());
                case ((int)ElementType.I2):
                    return new IntConst(ReadInt16());
                case ((int)ElementType.I4):
                    return new IntConst(ReadInt32());
                case ((int)ElementType.I8):
                    return new IntConst(ReadInt64());
                case ((int)ElementType.R4):
                    return new FloatConst(ReadSingle());
                case ((int)ElementType.R8):
                    return new DoubleConst(ReadDouble());
                case ((int)ElementType.String):
                    return new StringConst(ReadString());
                }
            } else if(constType is ManagedPointer) {
                uint dataRVA = ReadUInt32();
                Type baseType = ((ManagedPointer)constType).GetBaseType();
                return new AddressConstant(GetDataConstant(dataRVA, baseType));
            } // need to do repeated constant??
            return null;
        }

        internal ModuleFile GetFileDesc(string name)
        {
            if(tables[(int)MDTable.File] == null)
                return null;
            for(int i=0; i < tables[(int)MDTable.File].Length; i++) {
                FileRef fr = (FileRef)tables[(int)MDTable.File][i];
                if(fr.Name() == name) {
                    if(fr is ModuleFile)
                        return (ModuleFile)fr;
                    fr = new ModuleFile(fr.Name(), fr.GetHash());
                    tables[(int)MDTable.File][i] = fr;
                    return (ModuleFile)fr;
                }
            }
            return null;
        }

        /*
        private long GetOffset(int rva) {
          for (int i=0; i < inputSections.Length; i++) {
            long offs = inputSections[i].GetOffset(rva);
            if (offs > 0) return offs;
          }
          return 0;
        }

        public bool ReadPadding(int boundary) {
          while ((Position % boundary) != 0) {
            if (buffer[index++] != 0) { return false; }
          }
          return true;
        }

        public String ReadName() {
          int len = NAMELEN;
          char [] nameStr = new char[NAMELEN];
          char ch = (char)ReadByte();
          int i=0;
          for (; (i < NAMELEN) && (ch != '\0'); i++) {
            nameStr[i] = ch;
            ch = (char)ReadByte();
          }
          return new String(nameStr,0,i);
        }

        internal String ReadString() {
          char [] str = new char[STRLEN];
          int i=0;
          char ch = (char)ReadByte();
          for (; ch != '\0'; i++) {
            str[i] = ch;
            ch = (char)ReadByte();
          }
          return new String(str,0,i);
        }

        public long GetPos() {
          return BaseStream.Position;
        }

        public void SetPos(int ix) {
          BaseStream.Position = ix;
        }
    */
        /*
        public void SetToRVA(int rva) {
          index = PESection.GetOffset(rva);
    //      Console.WriteLine("Setting buffer to rva " + PEConsts.Hex(rva) + " = index " + PEConsts.Hex(index));
    //      Console.WriteLine("Setting buffer to rva " + rva + " = index " + index);
        }

        public byte[] GetBuffer() {
          return buffer;
        }
    */
        private CILInstruction[] DoByteCodes(uint len, MethodDef thisMeth)
        {
            uint pos = 0;
            ArrayList instrList = new ArrayList();
            //int instrIx = 0;
            while(pos < len) {
                uint offset = pos;
                uint opCode = ReadByte();
                pos++;
                IType iType = IType.op;
                if(opCode == 0xFE) {
                    uint ix = ReadByte();
                    pos++;
                    opCode = (opCode << 8) + ix;
                    iType = FileImage.longInstrMap[ix];
                } else
                    iType = FileImage.instrMap[opCode];
                if(Diag.DiagOn)
                    Console.WriteLine("Got instruction type " + iType);
                CILInstruction nextInstr = null;
                if(iType == IType.specialOp) {
                    pos += 4;
                    if(Diag.DiagOn)
                        Console.WriteLine("Got instruction " + Hex.Byte((int)opCode));
                    switch(opCode) {
                    case ((int)SpecialOp.ldc_i8):
                        nextInstr = new LongInstr((SpecialOp)opCode, ReadInt64());
                        pos += 4;
                        break;
                    case ((int)SpecialOp.ldc_r4):
                        nextInstr = new FloatInstr((SpecialOp)opCode, ReadSingle());
                        break;
                    case ((int)SpecialOp.ldc_r8):
                        nextInstr = new DoubleInstr((SpecialOp)opCode, ReadDouble());
                        pos += 4;
                        break;
                    case ((int)SpecialOp.calli):
                        nextInstr = new SigInstr((SpecialOp)opCode, (CalliSig)GetTokenElement(ReadUInt32()));
                        break;
                    case ((int)SpecialOp.Switch): // switch
                        uint count = ReadUInt32();
                        int[] offsets = new int[count];
                        for(uint i=0; i < count; i++)
                            offsets[i] = ReadInt32();
                        pos += (4 * count);
                        nextInstr = new SwitchInstr(offsets);
                        break;
                    case ((int)SpecialOp.ldstr): // ldstr
                        uint strIx = ReadUInt32();
                        strIx = strIx & FileImage.ElementMask;
                        nextInstr = new StringInstr((SpecialOp)opCode, userstring.GetUserString(strIx));
                        break;
                    case ((int)MethodOp.ldtoken):
                        MetaDataElement elem = GetTokenElement(ReadUInt32());
                        if(elem is Method)
                            nextInstr = new MethInstr((MethodOp)opCode, (Method)elem);
                        else if(elem is Field)
                            nextInstr = new FieldInstr((FieldOp)opCode, (Field)elem);
                        else
                            nextInstr =new TypeInstr((TypeOp)opCode, (Type)elem);
                        break;
                    }
                } else if(iType == IType.branchOp) {
                    if(Diag.DiagOn)
                        Console.WriteLine("Got instruction " + Hex.Byte((int)opCode));
                    if((opCode < 0x38) || (opCode == 0xDE)) { // br or leave.s
                        nextInstr = new BranchInstr(opCode, ReadSByte());
                        pos++;
                    } else {
                        nextInstr = new BranchInstr(opCode, ReadInt32());
                        pos += 4;
                    }
                } else {
                    if(Diag.DiagOn)
                        Console.Write(Hex.Byte((int)opCode));
                    switch(iType) {
                    case (IType.op):
                        if(Diag.DiagOn)
                            Console.WriteLine("Got instruction " + (Op)opCode);
                        nextInstr = new Instr((Op)opCode);
                        break;
                    case (IType.methOp):
                        if(Diag.DiagOn)
                            Console.WriteLine("Got instruction " + (MethodOp)opCode);
                        nextInstr = new MethInstr((MethodOp)opCode, (Method)GetTokenElement(ReadUInt32()));
                        pos += 4;
                        break;
                    case (IType.typeOp):
                        if(Diag.DiagOn)
                            Console.WriteLine("Got instruction " + (TypeOp)opCode);
                        uint ttok = ReadUInt32();
                        Type typeToken = (Type)GetTokenElement(ttok);
                        if(typeToken is GenericParTypeSpec)
                            typeToken = ((GenericParTypeSpec)typeToken).GetGenericParam(thisMeth);
                        nextInstr = new TypeInstr((TypeOp)opCode, typeToken);
                        pos += 4;
                        break;
                    case (IType.fieldOp):
                        if(Diag.DiagOn)
                            Console.WriteLine("Got instruction " + (FieldOp)opCode);
                        nextInstr = new FieldInstr((FieldOp)opCode, (Field)GetTokenElement(ReadUInt32()));
                        pos += 4;
                        break;
                    case (IType.int8Op):
                        nextInstr = new IntInstr((IntOp)opCode, ReadSByte());
                        pos++;
                        break;
                    case (IType.uint8Op):
                        nextInstr = new UIntInstr((IntOp)opCode, ReadByte());
                        pos++;
                        break;
                    case (IType.uint16Op):
                        nextInstr =new UIntInstr((IntOp)opCode, ReadUInt16());
                        pos++;
                        break;
                    case (IType.int32Op):
                        nextInstr =new IntInstr((IntOp)opCode, ReadInt32());
                        pos += 4;
                        break;
                    }
                }
                if(nextInstr != null)
                    nextInstr.Resolve();
                instrList.Add(nextInstr);
            }
            CILInstruction[] instrs = new CILInstruction[instrList.Count];
            for(int i=0; i < instrs.Length; i++) {
                instrs[i] = (CILInstruction)instrList[i];
            }
            return instrs;
        }

        public void ReadByteCodes(MethodDef meth, uint rva)
        {
            if(rva == 0)
                return;
            BaseStream.Seek(GetOffset(rva), SeekOrigin.Begin);
            CILInstructions instrs = meth.CreateCodeBuffer();
            uint formatByte = ReadByte();
            uint format = formatByte & 0x3;
            if(Diag.DiagOn)
                Console.WriteLine("code header format = " + Hex.Byte((int)formatByte));
            uint size = 0;
            if(format == CILInstructions.TinyFormat) {
                size = formatByte >> 2;
                if(Diag.DiagOn)
                    Console.WriteLine("Tiny Format, code size = " + size);
                instrs.SetAndResolveInstructions(DoByteCodes(size, meth));
            } else if(format == CILInstructions.FatFormat) {
                uint headerSize = ReadByte();
                bool initLocals = (formatByte & CILInstructions.InitLocals) != 0;
                bool moreSects = (formatByte & CILInstructions.MoreSects) != 0;
                meth.SetMaxStack((int)ReadUInt16());
                size = ReadUInt32();
                if(Diag.DiagOn)
                    Console.WriteLine("Fat Format, code size = " + size);
                uint locVarSig = ReadUInt32();
                CILInstruction[] instrList = this.DoByteCodes(size, meth);
                while(moreSects) {
                    // find next 4 byte boundary
                    long currPos = BaseStream.Position;
                    if(currPos % 4 != 0) {
                        long pad = 4 - (currPos % 4);
                        for(int p = 0; p < pad; p++)
                            ReadByte();
                    }
                    uint flags = ReadByte();
                    //while (flags == 0) flags = ReadByte();  // maximum of 3 to get 4 byte boundary??
                    moreSects = (flags & CILInstructions.SectMoreSects) != 0;
                    bool fatSect = (flags & CILInstructions.SectFatFormat) != 0;
                    if((flags & CILInstructions.EHTable) == 0)
                        throw new Exception("Section not an Exception Handler Table");
                    int sectLen = ReadByte() + (ReadByte() << 8) + (ReadByte() << 16);
                    int numClauses = sectLen - 4;
                    if(fatSect)
                        numClauses /= 24;
                    else
                        numClauses /= 12;
                    for(int i=0; i < numClauses; i++) {
                        EHClauseType eFlag;
                        if(fatSect)
                            eFlag = (EHClauseType)ReadUInt32();
                        else
                            eFlag = (EHClauseType)ReadUInt16();
                        uint tryOff = 0, tryLen = 0, hOff = 0, hLen = 0;
                        if(fatSect) {
                            tryOff = ReadUInt32();
                            tryLen = ReadUInt32();
                            hOff = ReadUInt32();
                            hLen = ReadUInt32();
                        } else {
                            tryOff = ReadUInt16();
                            tryLen = ReadByte();
                            hOff = ReadUInt16();
                            hLen = ReadByte();
                        }
                        EHClause ehClause = new EHClause(eFlag, tryOff, tryLen, hOff, hLen);
                        if(eFlag == EHClauseType.Exception)
                            ehClause.ClassToken(GetTokenElement(ReadUInt32()));
                        else
                            ehClause.FilterOffset(ReadUInt32());
                        instrs.AddEHClause(ehClause);
                    }
                }
                if(locVarSig != 0) {
                    LocalSig lSig = (LocalSig)GetTokenElement(locVarSig);
                    lSig.Resolve(this, meth);
                    meth.AddLocals(lSig.GetLocals(), initLocals);
                }
                instrs.SetAndResolveInstructions(instrList);
            } else {
                Console.WriteLine("byte code format error");
            }
        }

    }
    /**************************************************************************/
    // Class containing MetaData Constants
    /**************************************************************************/
    /// <summary>
    /// MetaData 
    ///   Root (20 bytes + UTF-8 Version String + quad align padding)
    ///   StreamHeaders (8 bytes + null terminated name string + quad align padding)
    ///   Streams 
    ///     #~        (always present - holds metadata tables)
    ///     #Strings  (always present - holds identifier strings)
    ///     #US       (Userstring heap)
    ///     #Blob     (signature blobs)
    ///     #GUID     (guids for assemblies or Modules)
    /// </summary>

    internal class MetaData {
        internal static readonly uint maxSmlIxSize = 0xFFFF;
        internal static readonly uint max1BitSmlIx = 0x7FFF;
        internal static readonly uint max2BitSmlIx = 0x3FFF;
        internal static readonly uint max3BitSmlIx = 0x1FFF;
        internal static readonly uint max5BitSmlIx = 0x7FF;
        internal static readonly uint[] CIxBitMasks = { 0x0, 0x0001, 0x0003, 0x0007, 0x000F, 0x001F };
        internal static readonly int[] CIxShiftMap = { 2, 2, 5, 1, 2, 3, 1, 1, 1, 2, 3, 2, 1 };
        internal static readonly uint[] CIxMaxMap = {max2BitSmlIx,max2BitSmlIx,max5BitSmlIx,
                                                  max1BitSmlIx,max2BitSmlIx,max3BitSmlIx,
                                                  max1BitSmlIx,max1BitSmlIx,max1BitSmlIx,
                                                  max2BitSmlIx,max3BitSmlIx,max2BitSmlIx, max1BitSmlIx};
        internal static readonly int[] TypeDefOrRefTable = { (int)MDTable.TypeDef, (int)MDTable.TypeRef, (int)MDTable.TypeSpec };
        internal static readonly int[] HasConstantTable = { (int)MDTable.Field, (int)MDTable.Param, (int)MDTable.Property };
        internal static readonly int[] HasCustomAttributeTable = {(int)MDTable.Method, (int)MDTable.Field, (int)MDTable.TypeRef, 
                                                               (int)MDTable.TypeDef,(int)MDTable.Param, (int)MDTable.InterfaceImpl,
                                                               (int)MDTable.MemberRef, (int)MDTable.Module,(int)MDTable.DeclSecurity,
                                                               (int)MDTable.Property,(int)MDTable.Event, (int)MDTable.StandAloneSig, 
                                                               (int)MDTable.ModuleRef, (int)MDTable.TypeSpec, (int)MDTable.Assembly,
                                                               (int)MDTable.AssemblyRef, (int)MDTable.File, (int)MDTable.ExportedType, 
                                                               (int)MDTable.ManifestResource };
        internal static readonly int[] HasFieldMarshalTable = { (int)MDTable.Field, (int)MDTable.Param };
        internal static readonly int[] HasDeclSecurityTable = { (int)MDTable.TypeDef, (int)MDTable.Method, (int)MDTable.Assembly };
        internal static readonly int[] MemberRefParentTable = {(int)MDTable.TypeDef, (int)MDTable.TypeRef, (int)MDTable.ModuleRef, (int)MDTable.Method, 
                                                            (int)MDTable.TypeSpec };
        internal static readonly int[] HasSemanticsTable = { (int)MDTable.Event, (int)MDTable.Property };
        internal static readonly int[] MethodDefOrRefTable = { (int)MDTable.Method, (int)MDTable.MemberRef };
        internal static readonly int[] MemberForwardedTable = { (int)MDTable.Field, (int)MDTable.Method };
        internal static readonly int[] ImplementationTable = { (int)MDTable.File, (int)MDTable.AssemblyRef, (int)MDTable.ExportedType };
        internal static readonly int[] CustomAttributeTypeTable = { 0, 0, (int)MDTable.Method, (int)MDTable.MemberRef };
        internal static readonly int[] ResolutionScopeTable = {(int)MDTable.Module, (int)MDTable.ModuleRef, (int)MDTable.AssemblyRef,
                                                            (int)MDTable.TypeRef };
        internal static readonly int[] TypeOrMethodDefTable = { (int)MDTable.TypeDef, (int)MDTable.Method };
        internal static readonly int[][] CIxTables = {TypeDefOrRefTable, HasConstantTable,
                                                   HasCustomAttributeTable, HasFieldMarshalTable, HasDeclSecurityTable, 
                                                   MemberRefParentTable, HasSemanticsTable, MethodDefOrRefTable, MemberForwardedTable,
                                                   ImplementationTable, CustomAttributeTypeTable, ResolutionScopeTable,
                                                   TypeOrMethodDefTable };

        internal static readonly byte StringsHeapMask = 0x1;
        internal static readonly byte GUIDHeapMask = 0x2;
        internal static readonly byte BlobHeapMask = 0x4;
        internal static readonly uint MetaDataSignature = 0x424A5342;
        // NOTE: version and stream name strings MUST always be quad padded
        internal static readonly string[] versions = { "v1.1.4322\0\0\0", "v2.0.40607\0\0", "v2.0.41202\0\0" };
        internal static readonly byte[] LMajors = { 6, 8, 8 };
        //internal static readonly string shortVersion = version.Substring(0,9);
        internal static readonly char[] tildeNameArray = { '#', '~', '\0', '\0' };
        internal static readonly char[] stringsNameArray = { '#', 'S', 't', 'r', 'i', 'n', 'g', 's', '\0', '\0', '\0', '\0' };
        internal static readonly char[] usNameArray = { '#', 'U', 'S', '\0' };
        internal static readonly char[] guidNameArray = { '#', 'G', 'U', 'I', 'D', '\0', '\0', '\0' };
        internal static readonly char[] blobNameArray = { '#', 'B', 'l', 'o', 'b', '\0', '\0', '\0' };
        internal static readonly String stringsName = "#Strings";
        internal static readonly String userstringName = "#US";
        internal static readonly String blobName = "#Blob";
        internal static readonly String guidName = "#GUID";
        internal static readonly String tildeName = "#~";
        internal static readonly uint MetaDataHeaderSize = 20 + (uint)versions[0].Length;
        internal static readonly uint TildeHeaderSize = 24;
        internal static readonly uint StreamHeaderSize = 8;
        internal static readonly uint NumMetaDataTables = (int)MDTable.MaxMDTable;
        internal static readonly uint tildeHeaderSize = 8 + (uint)tildeNameArray.Length;

        internal ulong valid = 0, /*sorted = 0x000002003301FA00;*/ sorted = 0;
        internal bool[] largeIx = new bool[NumMetaDataTables];
        internal bool[] lgeCIx = new bool[(int)CIx.MaxCIx];
        internal uint[] elemSize = new uint[NumMetaDataTables];
        internal bool largeStrings = false, largeUS = false, largeGUID = false, largeBlob = false;

        internal MetaData()
        {
            InitMetaData();
        }

        internal void InitMetaData()
        {
            for(int i=0; i < NumMetaDataTables; i++)
                largeIx[i] = false;
            for(int i=0; i < lgeCIx.Length; i++)
                lgeCIx[i] = false;
        }

        internal bool LargeIx(MDTable tabIx)
        {
            return largeIx[(uint)tabIx];
        }

        internal uint CodedIndexSize(CIx code)
        {
            if(lgeCIx[(uint)code])
                return 4;
            return 2;
        }

        internal uint TableIndexSize(MDTable tabIx)
        {
            if(largeIx[(uint)tabIx])
                return 4;
            return 2;
        }

        internal uint StringsIndexSize()
        {
            if(largeStrings)
                return 4;
            return 2;
        }

        internal uint GUIDIndexSize()
        {
            if(largeGUID)
                return 4;
            return 2;
        }

        internal uint USIndexSize()
        {
            if(largeUS)
                return 4;
            return 2;
        }

        internal uint BlobIndexSize()
        {
            if(largeBlob)
                return 4;
            return 2;
        }

        internal void CalcElemSize()
        {
            elemSize[(int)MDTable.Assembly] = Assembly.Size(this);
            elemSize[(int)MDTable.AssemblyOS] = 12;
            elemSize[(int)MDTable.AssemblyProcessor] = 4;
            elemSize[(int)MDTable.AssemblyRefOS] = 12 + TableIndexSize(MDTable.AssemblyRef);
            elemSize[(int)MDTable.AssemblyRefProcessor] = 4 + TableIndexSize(MDTable.AssemblyRef);
            elemSize[(int)MDTable.Module] = Module.Size(this);
            elemSize[(int)MDTable.TypeRef] = ClassRef.Size(this);
            elemSize[(int)MDTable.TypeDef] = ClassDef.Size(this);
            elemSize[(int)MDTable.Field] = FieldDef.Size(this);
            elemSize[(int)MDTable.Method] = MethodDef.Size(this);
            elemSize[(int)MDTable.Param] = Param.Size(this);
            elemSize[(int)MDTable.InterfaceImpl] = InterfaceImpl.Size(this);
            elemSize[(int)MDTable.MemberRef] = FieldRef.Size(this);
            elemSize[(int)MDTable.Constant] = ConstantElem.Size(this);
            elemSize[(int)MDTable.CustomAttribute] = CustomAttribute.Size(this);
            elemSize[(int)MDTable.FieldMarshal] = FieldMarshal.Size(this);
            elemSize[(int)MDTable.DeclSecurity] = DeclSecurity.Size(this);
            elemSize[(int)MDTable.ClassLayout] = ClassLayout.Size(this);
            elemSize[(int)MDTable.FieldLayout] = FieldLayout.Size(this);
            elemSize[(int)MDTable.StandAloneSig] = Signature.Size(this);
            elemSize[(int)MDTable.EventMap] = MapElem.Size(this, MDTable.EventMap);
            elemSize[(int)MDTable.Event] = Event.Size(this);
            elemSize[(int)MDTable.PropertyMap] = MapElem.Size(this, MDTable.PropertyMap);
            elemSize[(int)MDTable.Property] = Property.Size(this);
            elemSize[(int)MDTable.MethodSemantics] = MethodSemantics.Size(this);
            elemSize[(int)MDTable.MethodImpl] = MethodImpl.Size(this);
            elemSize[(int)MDTable.ModuleRef] = ModuleRef.Size(this);
            elemSize[(int)MDTable.TypeSpec] = TypeSpec.Size(this);
            elemSize[(int)MDTable.ImplMap] = ImplMap.Size(this);
            elemSize[(int)MDTable.FieldRVA] = FieldRVA.Size(this);
            elemSize[(int)MDTable.Assembly] = Assembly.Size(this);
            elemSize[(int)MDTable.AssemblyRef] = AssemblyRef.Size(this);
            elemSize[(int)MDTable.File] = FileRef.Size(this);
            elemSize[(int)MDTable.ExportedType] = ExternClass.Size(this);
            elemSize[(int)MDTable.ManifestResource] = ManifestResource.Size(this);
            elemSize[(int)MDTable.NestedClass] = MapElem.Size(this, MDTable.NestedClass);
            elemSize[(int)MDTable.GenericParam] = GenericParam.Size(this);
            elemSize[(int)MDTable.GenericParamConstraint] = GenericParamConstraint.Size(this);
            elemSize[(int)MDTable.MethodSpec] = MethodSpec.Size(this);
        }

    }
    /**************************************************************************/
    // MetaData generated from user created descriptors
    /**************************************************************************/
    internal class MetaDataOut : MetaData {
        MetaDataStream strings, us, guid, blob;
        MetaDataStream[] streams;
        uint numStreams = 5;
        uint tildeTide = 0, tildePadding = 0, tildeStart = 0;
        uint numTables = 0, resourcesSize = 0;
        ArrayList byteCodes = new ArrayList();
        uint codeSize = 0, byteCodePadding = 0, metaDataSize = 0;
        private PEWriter output;
        private byte heapSizes = 0;
        MetaDataElement entryPoint;
        long mdStart;
        ArrayList resources;
        private ArrayList[] tables = new ArrayList[NumMetaDataTables];

        internal MetaDataOut()
            : base()
        {
        }

        internal void InitMetaDataOut(PEWriter file)
        {
            // tilde = new MetaDataStream(tildeNameArray,false,0);
            this.output = file;
            streams = new MetaDataStream[5];
            strings = new MetaDataStream(MetaData.stringsNameArray, new UTF8Encoding(), true);
            us = new MetaDataStream(MetaData.usNameArray, new UnicodeEncoding(), true);
            guid = new MetaDataStream(MetaData.guidNameArray, false);
            blob = new MetaDataStream(MetaData.blobNameArray, new UnicodeEncoding(), true);
            streams[1] = strings;
            streams[2] = us;
            streams[3] = guid;
            streams[4] = blob;
        }

        internal uint Size()
        {
            //Console.WriteLine("metaData size = " + metaDataSize);
            return metaDataSize;
        }

        internal uint AddToUSHeap(string str)
        {
            if(str == null)
                return 0;
            return us.Add(str, true);
        }

        internal uint AddToStringsHeap(string str)
        {
            if((str == null) || (str == ""))
                return 0;
            return strings.Add(str, false);
        }

        internal uint AddToGUIDHeap(Guid guidNum)
        {
            return guid.Add(guidNum);
        }

        internal uint AddToBlobHeap(byte[] blobBytes)
        {
            if(blobBytes == null)
                return 0;
            return blob.Add(blobBytes);
        }

        internal uint AddToBlobHeap(long val, uint numBytes)
        {
            return blob.Add(val, numBytes);
        }

        internal uint AddToBlobHeap(ulong val, uint numBytes)
        {
            return blob.Add(val, numBytes);
        }

        internal uint AddToBlobHeap(char ch)
        {
            return blob.Add(ch);
        }

        /*
        internal uint AddToBlobHeap(byte val) {
          return blob.Add(val);
        }

        internal uint AddToBlobHeap(sbyte val) {
          return blob.Add(val);
        }

        internal uint AddToBlobHeap(ushort val) {
          return blob.Add(val);
        }

        internal uint AddToBlobHeap(short val) {
          return blob.Add(val);
        }

        internal uint AddToBlobHeap(uint val) {
          return blob.Add(val);
        }

        internal uint AddToBlobHeap(int val) {
          return blob.Add(val);
        }

        internal uint AddToBlobHeap(ulong val) {
          return blob.Add(val);
        }

        internal uint AddToBlobHeap(long val) {
          return blob.Add(val);
        }
        */

        internal uint AddToBlobHeap(float val)
        {
            return blob.Add(val);
        }

        internal uint AddToBlobHeap(double val)
        {
            return blob.Add(val);
        }

        internal uint AddToBlobHeap(string val)
        {
            return blob.Add(val, true);
        }

        private ArrayList GetTable(MDTable tableIx)
        {
            int tabIx = (int)tableIx;
            if(tables[tabIx] == null) {
                tables[tabIx] = new ArrayList();
                valid |= ((ulong)0x1 << tabIx);
                // Console.WriteLine("after creating table " + tableIx + "(" + tabIx + ") valid = " + valid);
                numTables++;
            }
            return tables[tabIx];
        }

        internal void AddToTable(MDTable tableIx, MetaDataElement elem)
        {
            // updates Row field of the element
            // Console.WriteLine("Adding element to table " + (uint)tableIx);
            ArrayList table = GetTable(tableIx);
            if(table.Contains(elem)) {
                Console.Out.WriteLine("ERROR - element already in table " + tableIx);
                return;
            }
            elem.Row = (uint)table.Count + 1;
            table.Add(elem);
        }

        internal uint TableIndex(MDTable tableIx)
        {
            if(tables[(int)tableIx] == null)
                return 1;
            return (uint)tables[(int)tableIx].Count+1;
        }

        internal uint AddCode(CILInstructions byteCode)
        {
            byteCodes.Add(byteCode);
            uint offset = codeSize;
            codeSize += byteCode.GetCodeSize();
            return offset;
        }

        internal void SetEntryPoint(MetaDataElement ep)
        {
            entryPoint = ep;
        }

        internal uint AddResource(byte[] resBytes)
        {
            if(resources == null)
                resources = new ArrayList();
            resources.Add(resBytes);
            uint offset = resourcesSize;
            resourcesSize += (uint)resBytes.Length + 4;
            return offset;
        }

        internal void AddData(DataConstant cVal)
        {
            output.AddInitData(cVal);
        }

        internal static void CompressNum(uint val, MemoryStream sig)
        {
            if(val <= 0x7F) {
                sig.WriteByte((byte)val);
            } else if(val <= 0x3FFF) {
                byte b1 = (byte)((val >> 8) | 0x80);
                byte b2 = (byte)(val & FileImage.iByteMask[0]);
                sig.WriteByte(b1);
                sig.WriteByte(b2);
            } else {
                byte b1 = (byte)((val >> 24) | 0xC0);
                byte b2 = (byte)((val & FileImage.iByteMask[2]) >> 16);
                byte b3 = (byte)((val & FileImage.iByteMask[1]) >> 8);
                ;
                byte b4 = (byte)(val & FileImage.iByteMask[0]);
                sig.WriteByte(b1);
                sig.WriteByte(b2);
                sig.WriteByte(b3);
                sig.WriteByte(b4);
            }
        }

        internal uint CodeSize()
        {
            return codeSize + byteCodePadding;
        }

        internal uint GetResourcesSize()
        {
            return resourcesSize;
        }

        private void SetStreamOffsets()
        {
            uint sizeOfHeaders = StreamHeaderSize + (uint)tildeNameArray.Length;
            for(int i=1; i < numStreams; i++) {
                sizeOfHeaders += streams[i].headerSize();
            }
            metaDataSize = MetaDataHeaderSize + sizeOfHeaders;
            // Console.WriteLine("Size of meta data headers (tildeStart) = " + metaDataSize);
            tildeStart = metaDataSize;
            metaDataSize += tildeTide + tildePadding;
            //Console.WriteLine(tildeNameArray + " - size = " + (tildeTide + tildePadding));
            for(int i=1; i < numStreams; i++) {
                // Console.WriteLine("Stream " + i + " starts at " + metaDataSize);
                streams[i].Start = metaDataSize;
                metaDataSize += streams[i].Size();
                streams[i].WriteDetails();
            }
            largeStrings = strings.LargeIx();
            largeBlob = blob.LargeIx();
            largeGUID = guid.LargeIx();
            largeUS = us.LargeIx();
        }

        internal void CalcTildeStreamSize()
        {
            CalcElemSize();
            //tilde.SetIndexSizes(strings.LargeIx(),us.LargeIx(),guid.LargeIx(),blob.LargeIx());
            tildeTide = TildeHeaderSize;
            tildeTide += 4 * numTables;
            //Console.WriteLine("Tilde header + sizes = " + tildeTide);
            for(int i=0; i < NumMetaDataTables; i++) {
                if(tables[i] != null) {
                    ArrayList table = tables[i];
                    // Console.WriteLine("Meta data table " + i + " at offset " + tildeTide);
                    tildeTide += (uint)table.Count * elemSize[i];
                    // Console.WriteLine("Metadata table " + i + " has size " + table.Count);
                    // Console.WriteLine("tildeTide = " + tildeTide);
                }
            }
            if((tildeTide % 4) != 0)
                tildePadding = 4 - (tildeTide % 4);
            //Console.WriteLine("tildePadding = " + tildePadding);
        }

        internal void WriteTildeStream(PEWriter output)
        {
            long startTilde = output.Seek(0, SeekOrigin.Current);
            //Console.WriteLine("Starting tilde output at offset " + Hex.Long(startTilde));
            output.Write((uint)0); // Reserved
            output.Write(output.verInfo.tsMajVer); // MajorVersion
            output.Write(output.verInfo.tsMinVer); // MinorVersion
            output.Write(heapSizes);
            output.Write((byte)1); // Reserved
            output.Write(valid);
            output.Write(sorted);
            for(int i=0; i < NumMetaDataTables; i++) {
                if(tables[i] != null) {
                    uint count = (uint)tables[i].Count;
                    output.Write(count);
                }
            }
            long tabStart = output.Seek(0, SeekOrigin.Current);
            // Console.WriteLine("Starting metaData tables at " + tabStart);
            for(int i=0; i < NumMetaDataTables; i++) {
                if(tables[i] != null) {
                    //Console.WriteLine("Starting metaData table " + i + " at " + (output.Seek(0,SeekOrigin.Current) - startTilde));
                    ArrayList table = tables[i];
                    for(int j=0; j < table.Count; j++) {
                        ((MetaDataElement)table[j]).Write(output);
                    }
                }
            }
            //Console.WriteLine("Writing padding at " + output.Seek(0,SeekOrigin.Current));
            for(int i=0; i < tildePadding; i++)
                output.Write((byte)0);
        }

        private void SortTable(ArrayList mTable)
        {
            //Console.WriteLine("Sorting table");
            if(mTable == null)
                return;
            mTable.Sort();
            for(int i=0; i < mTable.Count; i++) {
                ((MetaDataElement)mTable[i]).Row = (uint)i+1;
            }
        }

        internal void BuildMDTables()
        {
            // Check ordering of specific tables
            // Constant, CustomAttribute, FieldMarshal, DeclSecurity, MethodSemantics
            // ImplMap, GenericParam
            // Need to load GenericParamConstraint AFTER GenericParam table in correct order
            // The tables:
            //   InterfaceImpl, ClassLayout, FieldLayout, MethodImpl, FieldRVA, NestedClass
            // will _ALWAYS_ be in the correct order as embedded in BuildMDTables

            SortTable(tables[(int)MDTable.Constant]);
            SortTable(tables[(int)MDTable.CustomAttribute]);
            SortTable(tables[(int)MDTable.FieldMarshal]);
            SortTable(tables[(int)MDTable.DeclSecurity]);
            SortTable(tables[(int)MDTable.MethodSemantics]);
            SortTable(tables[(int)MDTable.ImplMap]);
            if(tables[(int)MDTable.GenericParam] != null) {
                SortTable(tables[(int)MDTable.GenericParam]);
                // Now add GenericParamConstraints
                for(int i=0; i < tables[(int)MDTable.GenericParam].Count; i++) {
                    ((GenericParam)tables[(int)MDTable.GenericParam][i]).AddConstraints(this);
                }
            }

            /*
            // for bug in Whidbey GenericParam table ordering
            int end = tables[(int)MDTable.TypeDef].Count;
            int methEnd = 0;
            if (tables[(int)MDTable.Method] != null) {
              methEnd = tables[(int)MDTable.Method].Count;
            }
            for (int i=0; i < end; i++) {
              ((ClassDef)tables[(int)MDTable.TypeDef][i]).AddGenericsToTable(this);
              if (methEnd > i)
                ((MethodDef)tables[(int)MDTable.Method][i]).AddGenericsToTable(this);
            }
            for (int i=end; i < methEnd; i++) {
              ((MethodDef)tables[(int)MDTable.Method][i]).AddGenericsToTable(this);
            }
            // end of bug fix
            */
            for(int i=0; i < tables.Length; i++) {
                if(tables[i] != null) {
                    for(int j=0; j < tables[i].Count; j++) {
                        ((MetaDataElement)tables[i][j]).BuildSignatures(this);
                    }
                }
            }
        }

        internal void SetIndexSizes()
        {
            for(int i=0; i < NumMetaDataTables; i++) {
                if(tables[i] != null) {
                    largeIx[i] = (uint)tables[i].Count > maxSmlIxSize;
                }
            }
            for(int i=0; i < CIxTables.Length; i++) {
                for(int j=0; j < CIxTables[i].Length; j++) {
                    int tabIx = CIxTables[i][j];
                    if(tables[tabIx] != null) {
                        lgeCIx[i] = lgeCIx[i] | tables[tabIx].Count > CIxMaxMap[i];
                    }
                }
            }
        }

        internal void BuildMetaData()
        {
            SetIndexSizes();
            for(int i=1; i < numStreams; i++) {
                if(streams[i].Size() <= 1) {
                    //Console.WriteLine("Stream " + new String(streams[i].name) + " has size 0");
                    for(int j=i+1; j < numStreams; j++) {
                        streams[i] = streams[j];
                    }
                    i--;
                    numStreams--;
                } else
                    streams[i].EndStream();
            }
            //Console.WriteLine("numStreams = " + numStreams);
            CalcTildeStreamSize();
            SetStreamOffsets();
            byteCodePadding = NumToAlign(codeSize, 4);
            if(entryPoint != null)
                output.SetEntryPoint(entryPoint.Token());
        }

        internal void WriteByteCodes(PEWriter output)
        {
            for(int i=0; i < byteCodes.Count; i++) {
                ((CILInstructions)byteCodes[i]).Write(output);
            }
            for(int i=0; i < byteCodePadding; i++) {
                output.Write((byte)0);
            }
        }

        internal void WriteResources(PEWriter output)
        {
            if(resources == null)
                return;
            for(int i=0; i < resources.Count; i++) {
                byte[] resBytes = (byte[])resources[i];
                output.Write((uint)resBytes.Length);
                output.Write(resBytes);
            }
        }

        internal void WriteMetaData(PEWriter output)
        {
            this.output = output;
            if(Diag.DiagOn) {
                mdStart = output.Seek(0, SeekOrigin.Current);
                Console.WriteLine("Writing metaData at " + Hex.Long(mdStart));
            }
            output.Write(MetaDataSignature);
            output.Write(output.verInfo.mdMajVer);
            output.Write(output.verInfo.mdMinVer);
            output.Write(0);         // Reserved
            output.Write(output.verInfo.netVerString.Length);
            output.Write(output.verInfo.netVerString.ToCharArray());   // version string is already zero padded
            output.Write((short)0);  // Flags, reserved
            output.Write((ushort)numStreams);
            // write tilde header
            output.Write(tildeStart);
            output.Write(tildeTide + tildePadding);
            output.Write(tildeNameArray);
            for(int i=1; i < numStreams; i++)
                streams[i].WriteHeader(output);
            if(Diag.DiagOn)
                Console.WriteLine("Writing tilde stream at " + Hex.Long(output.Seek(0, SeekOrigin.Current)) + " = " + Hex.Int(tildeStart));
            WriteTildeStream(output);
            for(int i=1; i < numStreams; i++)
                streams[i].Write(output);
            // Console.WriteLine("Finished Writing metaData at " + output.Seek(0,SeekOrigin.Current));
        }

        //    internal bool LargeStringsIndex() { return strings.LargeIx(); }
        //    internal bool LargeGUIDIndex() { return guid.LargeIx(); }
        //    internal bool LargeUSIndex() { return us.LargeIx(); }
        //    internal bool LargeBlobIndex() { return blob.LargeIx(); }

        private uint NumToAlign(uint val, uint alignVal)
        {
            if((val % alignVal) == 0)
                return 0;
            return alignVal - (val % alignVal);
        }

        internal void WriteCodedIndex(CIx code, MetaDataElement elem, PEWriter output)
        {
            uint ix = 0;
            if(elem != null) {
                ix = (elem.Row << CIxShiftMap[(uint)code]) | elem.GetCodedIx(code);
                // Console.WriteLine("coded index = " + ix + " row = " + elem.Row);
                //} else {
                // Console.WriteLine("elem for coded index is null");
            }
            if(lgeCIx[(uint)code])
                output.Write(ix);
            else
                output.Write((ushort)ix);
        }

    }
    /**************************************************************************/
    // Streams for PE File Reader
    /**************************************************************************/
    /// <summary>
    /// Stream in the Meta Data  (#Strings, #US, #Blob and #GUID)
    /// </summary>
    /// 

    internal class MetaDataInStream : BinaryReader {
        //protected bool largeIx = false;
        protected byte[] data;

        public MetaDataInStream(byte[] streamBytes)
            : base(new MemoryStream(streamBytes))
        {
            data = streamBytes;
        }

        /*
        public bool LargeIx {
          get { return largeIx; }
          set { largeIx = value; }
        }
        */

        public uint ReadCompressedNum()
        {
            //int pos = (int)BaseStream.Position;
            //Console.WriteLine("Position = " + BaseStream.Position);
            byte b = ReadByte();
            //pos++;
            uint num = 0;
            if(b <= 0x7F) {
                num = b;
                //Console.WriteLine("Bytes = " + b);
            } else if(b >= 0xC0) {
                num = (uint)(((b - 0xC0) << 24) + (ReadByte() << 16) + (ReadByte() << 8) + ReadByte());
                //Console.WriteLine("Bytes = " + b + " " + Hex.Byte(data[pos++]) + " " + Hex.Byte(data[pos++]) + " " + Hex.Byte(data[pos++]));
            } else { // (b >= 0x80) && (b < 0xC0)
                num = (uint)((b - 0x80) << 8) + ReadByte();
                //Console.WriteLine("Bytes = " + b + " " + Hex.Byte(data[pos++]));
            }
            //Console.WriteLine("Compressed Num = " + num);
            return num;
        }

        internal bool AtEnd()
        {
            long pos = BaseStream.Position;
            long len = BaseStream.Length;
            //if (pos >= len-1)
            //  Console.WriteLine("At end of stream");
            return BaseStream.Position == BaseStream.Length-1;
        }

        internal byte[] GetBlob(uint ix)
        {
            if(ix == 0)
                return new byte[0];
            BaseStream.Seek(ix, SeekOrigin.Begin);
            //Console.WriteLine("Getting blob size at index " + buff.GetPos());
            //if (Diag.CADiag) Console.WriteLine("Getting blob size at " + (BaseStream.Position+PEReader.blobStreamStartOffset));
            uint bSiz = ReadCompressedNum();
            //byte[] blobBytes = new byte[ReadCompressedNum()];
            //if (Diag.CADiag) Console.WriteLine("Blob size =  " + bSiz);
            byte[] blobBytes = new byte[bSiz];
            for(int i=0; i < blobBytes.Length; i++) {
                blobBytes[i] = ReadByte();
            }
            return blobBytes;
        }

        internal byte[] GetBlob(uint ix, int len)
        {
            //Console.WriteLine("Getting blob size at index " + buffer.GetPos());
            byte[] blobBytes = new byte[len];
            for(int i=0; i < len; i++) {
                blobBytes[i] = data[ix++];
            }
            return blobBytes;
        }

        internal string GetString(uint ix)
        {
            uint end;
            for(end = ix; data[end] != '\0'; end++)
                ;
            char[] str = new char[end-ix];
            for(int i=0; i < str.Length; i++) {
                str[i] = (char)data[ix+i];
            }
            return new string(str, 0, str.Length);
        }

        internal string GetBlobString(uint ix)
        {
            if(ix == 0)
                return "";
            BaseStream.Seek(ix, SeekOrigin.Begin);
            return GetBlobString();
        }

        internal string GetBlobString()
        {
            uint strLen = ReadCompressedNum();
            char[] str = new char[strLen];
            uint readpos = (uint)this.BaseStream.Position;
            for(int i=0; i < strLen; i++) {
                str[i] = ReadChar();
                uint newpos = (uint)this.BaseStream.Position;
                if(newpos > readpos+1)
                    strLen -= newpos-(readpos+1);
                readpos = newpos;
            }
            return new string(str, 0, (int)strLen);
        }

        /*
         *     internal uint ReadCompressedNum() {
              int pos = (int)BaseStream.Position;
              Console.WriteLine("Position = " + BaseStream.Position);
              uint num = (uint)Read7BitEncodedInt();
              Console.WriteLine("Compressed Num = " + num);
              Console.WriteLine("Bytes = " + Hex.Byte(data[pos++]) + " " + Hex.Byte(data[pos++]) + " " + Hex.Byte(data[pos++]) + " " + Hex.Byte(data[pos++]));
              return num;
            }
            */

        internal void GoToIndex(uint ix)
        {
            BaseStream.Seek(ix, SeekOrigin.Begin);
        }

    }
    /**************************************************************************/

    internal class MetaDataStringStream : BinaryReader {
        //BinaryReader br;

        internal MetaDataStringStream(byte[] bytes)
            : base(new MemoryStream(bytes), Encoding.Unicode)
        {
            //br = new BinaryReader(new MemoryStream(bytes)/*,Encoding.Unicode*/);
        }

        private uint GetStringLength()
        {
            uint b = ReadByte();
            uint num = 0;
            if(b <= 0x7F) {
                num = b;
            } else if(b >= 0xC0) {
                num = (uint)(((b - 0xC0) << 24) + (ReadByte() << 16) + (ReadByte() << 8) + ReadByte());
            } else { // (b >= 0x80) && (b < 0xC0)
                num = (uint)((b - 0x80) << 8) + ReadByte();
            }
            return num;
        }

        internal string GetUserString(uint ix)
        {
            BaseStream.Seek(ix, SeekOrigin.Begin);
            uint strLen = GetStringLength()/2;
            char[] strArray = new char[strLen];
            for(int i=0; i < strLen; i++) {
                //strArray[i] = ReadChar(); // works for everett but not whidbey
                strArray[i] = (char)ReadUInt16();
            }
            return new String(strArray);
        }

    }
    /**************************************************************************/
    // PE File Section Descriptor
    /**************************************************************************/
    /// <summary>
    /// Descriptor for a Section in a PEFile  eg .text, .sdata
    /// </summary>
    internal class Section {
        internal static readonly uint relocPageSize = 4096;  // 4K pages for fixups

        char[] name;
        string nameString;
        uint offset = 0, tide = 0, size = 0, rva = 0, relocTide = 0, numRelocs = 0;
        uint relocOff = 0, relocRVA = 0, lineRVA = 0, numLineNums = 0;
        uint flags = 0, padding = 0;
        uint[] relocs;
        //bool relocsDone = false;

        internal Section(string sName, uint sFlags)
        {
            nameString = sName;
            name = sName.ToCharArray();
            flags = sFlags;
        }

        internal Section(PEReader input)
        {
            name = new char[8];
            for(int i=0; i < name.Length; i++)
                name[i] = (char)input.ReadByte();
            nameString = new String(name);
            tide = input.ReadUInt32();
            rva = input.ReadUInt32();
            size = input.ReadUInt32();
            offset = input.ReadUInt32();
            relocRVA = input.ReadUInt32();
            lineRVA = input.ReadUInt32();
            numRelocs = input.ReadUInt16();
            numLineNums = input.ReadUInt16();
            flags = input.ReadUInt32();
            if(Diag.DiagOn) {
                Console.WriteLine("  " + nameString + " RVA = " + Hex.Int(rva) + "  vSize = " + Hex.Int(tide));
                Console.WriteLine("        FileOffset = " + Hex.Int(offset) + "  aSize = " + Hex.Int(size));
            }
        }

        /*
        internal void ReadRelocs(PEReader input) {
          if (numRelocs == 0) return;
          Section relocSect = input.GetSection(relocRVA);
          relocSect.GetRelocs(input, relocRVA);
         }

        internal void GetRelocs(PEReader input) {
          if (relocsDone) return;
          relocsDone = true;
          uint pageRVA = input.ReadUInt32();
          while (pageRVA != 0) {
            uint blockSize = input.ReadUInt32();
            Section sect = input.GetSection(pageRVA);
            for (int i=0; i < ((blockSize-8)/2); i++) {
              sect.AddTypedReloc(input.ReadUInt16());
            }
            pageRVA = input.ReadUInt32();
          }
        }
        */

        internal bool ContainsRVA(uint rvaPos)
        {
            return (rva <= rvaPos) && (rvaPos <= rva+tide);
        }
        internal uint GetOffset(uint inRVA)
        {
            uint offs = 0;
            if((rva <= inRVA) && (inRVA <= rva+tide))
                offs = offset + (inRVA - rva);
            return offs;
        }
        internal uint Tide()
        {
            return tide;
        }

        internal void IncTide(uint incVal)
        {
            tide += incVal;
        }

        internal uint Padding()
        {
            return padding;
        }

        internal uint Size()
        {
            return size;
        }

        internal void SetSize(uint pad)
        {
            padding = pad;
            size = tide + padding;
        }

        internal uint RVA()
        {
            return rva;
        }

        internal void SetRVA(uint rva)
        {
            this.rva = rva;
        }

        internal uint Offset()
        {
            return offset;
        }

        internal void SetOffset(uint offs)
        {
            offset = offs;
        }

        internal void DoBlock(BinaryWriter reloc, uint page, int start, int end)
        {
            //Console.WriteLine("rva = " + rva + "  page = " + page);
            if(Diag.DiagOn)
                Console.WriteLine("writing reloc block at " + reloc.BaseStream.Position);
            reloc.Write(rva + page);
            uint blockSize = (uint)(((end-start+1)*2) + 8);
            reloc.Write(blockSize);
            if(Diag.DiagOn)
                Console.WriteLine("Block size = " + blockSize);
            for(int j=start; j < end; j++) {
                //Console.WriteLine("reloc offset = " + relocs[j]);
                reloc.Write((ushort)((0x3 << 12) | (relocs[j] - page)));
            }
            reloc.Write((ushort)0);
            if(Diag.DiagOn)
                Console.WriteLine("finished reloc block at " + reloc.BaseStream.Position);
        }

        internal void DoRelocs(BinaryWriter reloc)
        {
            if(relocTide > 0) {
                // align block to 32 bit boundary
                relocOff = (uint)reloc.Seek(0, SeekOrigin.Current);
                if((relocOff % 32) != 0) {
                    uint padding = 32 - (relocOff % 32);
                    for(int i=0; i < padding; i++)
                        reloc.Write((byte)0);
                    relocOff += padding;
                }
                uint block = (relocs[0]/relocPageSize + 1) * relocPageSize;
                int start = 0;
                for(int i=1; i < relocTide; i++) {
                    if(relocs[i] >= block) {
                        DoBlock(reloc, block-relocPageSize, start, i);
                        start = i;
                        block = (relocs[i]/relocPageSize + 1) * relocPageSize;
                    }
                }
                DoBlock(reloc, block-relocPageSize, start, (int)relocTide);
            }
        }


        internal void AddReloc(uint offs)
        {
            if(Diag.DiagOn)
                Console.WriteLine("Adding a reloc to " + nameString + " section");
            int pos = 0;
            if(relocs == null) {
                relocs = new uint[5];
            } else {
                if(relocTide >= relocs.Length) {
                    uint[] tmp = relocs;
                    relocs = new uint[tmp.Length + 5];
                    for(int i=0; i < relocTide; i++) {
                        relocs[i] = tmp[i];
                    }
                }
                while((pos < relocTide) && (relocs[pos] < offs))
                    pos++;
                for(int i=pos; i < relocTide; i++) {
                    relocs[i+1] = relocs[i];
                }
            }
            relocs[pos] = offs;
            relocTide++;
            if(Diag.DiagOn)
                Console.WriteLine("relocTide = " + relocTide);
        }

        internal void WriteHeader(BinaryWriter output, uint relocRVA)
        {
            if(Diag.DiagOn)
                Console.WriteLine("relocTide = " + relocTide);
            output.Write(name);
            output.Write(tide);                 // Virtual size
            output.Write(rva);                  // Virtual address
            output.Write(size);                 // SizeOfRawData
            output.Write(offset);               // PointerToRawData
            if(relocTide > 0) {
                output.Write(relocRVA + relocOff);
            } else {
                if(Diag.DiagOn)
                    Console.WriteLine(nameString + " section has no relocs");
                output.Write(0);
            }                                   // PointerToRelocations
            output.Write(0);                    // PointerToLineNumbers
            output.Write((ushort)relocTide);    // NumberOfRelocations
            output.Write((ushort)0);            // NumberOfLineNumbers
            output.Write(flags);                // Characteristics
        }

    }
    /**************************************************************************/
    // Streams for generated MetaData
    /**************************************************************************/
    /// <summary>
    /// Stream in the generated Meta Data  (#Strings, #US, #Blob and #GUID)
    /// </summary>
    internal class MetaDataStream : BinaryWriter {
        private static readonly uint StreamHeaderSize = 8;
        private static uint maxSmlIxSize = 0xFFFF;

        private uint start = 0;
        uint size = 0, tide = 1;
        bool largeIx = false;
        uint sizeOfHeader;
        internal char[] name;
        Hashtable htable = new Hashtable();

        internal MetaDataStream(char[] name, bool addInitByte)
            : base(new MemoryStream())
        {
            if(addInitByte) {
                Write((byte)0);
                size = 1;
            }
            this.name = name;
            sizeOfHeader = StreamHeaderSize + (uint)name.Length;
        }

        internal MetaDataStream(char[] name, System.Text.Encoding enc, bool addInitByte)
            : base(new MemoryStream(), enc)
        {
            if(addInitByte) {
                Write((byte)0);
                size = 1;
            }
            this.name = name;
            sizeOfHeader = StreamHeaderSize + (uint)name.Length;
        }

        public uint Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
            }
        }

        internal uint headerSize()
        {
            // Console.WriteLine(name + " stream has headersize of " + sizeOfHeader);
            return sizeOfHeader;
        }

        //internal void SetSize(uint siz) {
        //  size = siz;
        //}

        internal uint Size()
        {
            return size;
        }

        internal bool LargeIx()
        {
            return largeIx;
        }

        internal void WriteDetails()
        {
            // Console.WriteLine(name + " - size = " + size);
        }

        internal uint Add(string str, bool prependSize)
        {
            Object val = htable[str];
            uint index = 0;
            if(val == null) {
                index = size;
                htable[str] = index;
                char[] arr = str.ToCharArray();
                if(prependSize)
                    CompressNum((uint)arr.Length*2+1);
                Write(arr);
                Write((byte)0);
                size = (uint)Seek(0, SeekOrigin.Current);
            } else {
                index = (uint)val;
            }
            return index;
        }

        internal uint Add(Guid guid)
        {
            Write(guid.ToByteArray());
            size =(uint)Seek(0, SeekOrigin.Current);
            return tide++;
        }

        internal uint Add(byte[] blob)
        {
            uint ix = size;
            CompressNum((uint)blob.Length);
            Write(blob);
            size = (uint)Seek(0, SeekOrigin.Current);
            return ix;
        }

        internal uint Add(long val, uint numBytes)
        {
            uint ix = size;
            Write((byte)numBytes);
            switch(numBytes) {
            case 1:
                Write((byte)val);
                break;
            case 2:
                Write((short)val);
                break;
            case 4:
                Write((int)val);
                break;
            default:
                Write(val);
                break;
            }
            size = (uint)Seek(0, SeekOrigin.Current);
            return ix;
        }

        internal uint Add(ulong val, uint numBytes)
        {
            uint ix = size;
            Write((byte)numBytes);
            switch(numBytes) {
            case 1:
                Write((byte)val);
                break;
            case 2:
                Write((ushort)val);
                break;
            case 4:
                Write((uint)val);
                break;
            default:
                Write(val);
                break;
            }
            size = (uint)Seek(0, SeekOrigin.Current);
            return ix;
        }
        /*
        internal uint Add(byte val) {
          uint ix = size;
          Write((byte)1);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }

        internal uint Add(sbyte val) {
          uint ix = size;
          Write((byte)1);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }

        internal uint Add(ushort val) {
          uint ix = size;
          Write((byte)2);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }

        internal uint Add(short val) {
          uint ix = size;
          Write((byte)2);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }

        internal uint Add(uint val) {
          uint ix = size;
          Write((byte)4);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }

        internal uint Add(int val) {
          uint ix = size;
          Write((byte)4);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }

        internal uint Add(ulong val) {
          uint ix = size;
          Write((byte)8);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }

        internal uint Add(long val) {
          uint ix = size;
          Write((byte)8);  // size of blob to follow
          Write(val);
          size = (uint)Seek(0,SeekOrigin.Current);
          return ix;
        }
        */

        internal uint Add(char ch)
        {
            uint ix = size;
            Write((byte)2);  // size of blob to follow
            Write(ch);
            size = (uint)Seek(0, SeekOrigin.Current);
            return ix;
        }

        internal uint Add(float val)
        {
            uint ix = size;
            Write((byte)4);  // size of blob to follow
            Write(val);
            size = (uint)Seek(0, SeekOrigin.Current);
            return ix;
        }

        internal uint Add(double val)
        {
            uint ix = size;
            Write((byte)8);  // size of blob to follow
            Write(val);
            size = (uint)Seek(0, SeekOrigin.Current);
            return ix;
        }

        private void CompressNum(uint val)
        {
            if(val <= 0x7F) {
                Write((byte)val);
            } else if(val <= 0x3FFF) {
                byte b1 = (byte)((val >> 8) | 0x80);
                byte b2 = (byte)(val & FileImage.iByteMask[0]);
                Write(b1);
                Write(b2);
            } else {
                byte b1 = (byte)((val >> 24) | 0xC0);
                byte b2 = (byte)((val & FileImage.iByteMask[2]) >> 16);
                byte b3 = (byte)((val & FileImage.iByteMask[1]) >> 8);
                ;
                byte b4 = (byte)(val & FileImage.iByteMask[0]);
                Write(b1);
                Write(b2);
                Write(b3);
                Write(b4);
            }
        }

        private void QuadAlign()
        {
            if((size % 4) != 0) {
                uint pad = 4 - (size % 4);
                size += pad;
                for(int i=0; i < pad; i++) {
                    Write((byte)0);
                }
            }
        }

        internal void EndStream()
        {
            QuadAlign();
            if(size > maxSmlIxSize) {
                largeIx = true;
            }
        }

        internal void WriteHeader(BinaryWriter output)
        {
            output.Write(start);
            output.Write(size);
            output.Write(name);
        }

        internal virtual void Write(BinaryWriter output)
        {
            // Console.WriteLine("Writing " + name + " stream at " + output.Seek(0,SeekOrigin.Current) + " = " + start);
            MemoryStream str = (MemoryStream)BaseStream;
            output.Write(str.ToArray());
        }

    }

    /**************************************************************************/
    public class MethSig {
        internal string name;
        internal CallConv callConv = CallConv.Default;
        internal Type retType;
        internal Type[] parTypes, optParTypes;
        internal uint numPars = 0, numOptPars = 0, numGenPars = 0;
        //uint sigIx;

        /*-------------------- Constructors ---------------------------------*/

        internal MethSig(string nam)
        {
            name = nam;
        }

        internal MethSig InstantiateGenTypes(Class classType, Type[] genTypes)
        {
            MethSig newSig = new MethSig(name);
            newSig.callConv = callConv;
            newSig.numPars = numPars;
            newSig.numOptPars = numOptPars;
            newSig.numGenPars = numGenPars;
            newSig.parTypes = ReplaceGenPars(parTypes, classType, genTypes);
            newSig.optParTypes = ReplaceGenPars(optParTypes, classType, genTypes);
            newSig.retType = SubstituteType(retType, classType, genTypes);
            return newSig;
        }

        private Type[] ReplaceGenPars(Type[] typeList, MetaDataElement paren, Type[] genTypes)
        {
            if(typeList == null)
                return null;
            Type[] newList = new Type[typeList.Length];
            for(int i=0; i < typeList.Length; i++) {
                newList[i] = SubstituteType(typeList[i], paren, genTypes);
            }
            return newList;
        }

        private Type SubstituteType(Type origType, MetaDataElement paren, Type[] genTypes)
        {
            if((origType is GenericParam) && (((GenericParam)origType).GetParent() == paren))
                return genTypes[((GenericParam)origType).Index];
            return origType;
        }

        internal void SetParTypes(Param[] parList)
        {
            if(parList == null) {
                numPars = 0;
                return;
            }
            numPars = (uint)parList.Length;
            parTypes = new Type[numPars];
            for(int i=0; i < numPars; i++) {
                parTypes[i] = parList[i].GetParType();
            }
        }

        internal void ChangeParTypes(ClassDef newType, ClassDef[] oldTypes)
        {
            System.Diagnostics.Debug.Assert(newType != null);
            for(int i=0; i < oldTypes.Length; i++) {
                if(retType == oldTypes[i])
                    retType = newType;
                for(int j=0; j < numPars; j++) {
                    if(parTypes[j] == oldTypes[i])
                        parTypes[j] = newType;
                }
                for(int j=0; j < numOptPars; j++) {
                    if(optParTypes[j] == oldTypes[i])
                        optParTypes[j] = newType;
                }
            }
        }

        /*
         *     internal bool HasNameAndSig(string name, Type[] sigTypes) {
              if (this.name.CompareTo(name) != 0) return false;
              return HasSig(sigTypes);
            }

            internal bool HasNameAndSig(string name, Type[] sigTypes, Type[] optTypes) {
              if (this.name.CompareTo(name) != 0) return false;
              return HasSig(sigTypes,optTypes);
            }
            */

        internal bool HasSig(Type[] sigTypes)
        {
            if(sigTypes == null)
                return (numPars == 0);
            if(sigTypes.Length != numPars)
                return false;
            for(int i=0; i < numPars; i++) {
                if(!sigTypes[i].SameType(parTypes[i]))
                    return false;
            }
            return (optParTypes == null) || (optParTypes.Length == 0);
        }

        internal bool HasSig(Type[] sigTypes, Type[] optTypes)
        {
            if(sigTypes == null) {
                if(numPars > 0)
                    return false;
                if(optTypes == null)
                    return (numOptPars == 0);
            }
            if(sigTypes.Length != numPars)
                return false;
            for(int i=0; i < numPars; i++) {
                if(!sigTypes[i].SameType(parTypes[i]))
                    return false;
            }
            if(optTypes == null)
                return numOptPars == 0;
            if(optTypes.Length != numOptPars)
                return false;
            for(int i=0; i < optTypes.Length; i++) {
                if(!optTypes[i].SameType(optParTypes[i]))
                    return false;
            }
            return true;
        }
        /*    
           internal void CheckParTypes(Param[] parList) {
             //numGenPars = 0;
             for (int i=0; i < numPars; i++) {
               if (parTypes[i] is GenericParam)
                 numGenPars++;
             }
             if (numGenPars > 0)
               callConv |= CallConv.Generic;
             else if ((callConv & CallConv.Generic) > 0)
               callConv ^= CallConv.Generic;
           }
           */

        internal void TypeSig(MemoryStream sig)
        {
            sig.WriteByte((byte)callConv);
            if(numGenPars > 0)
                MetaDataOut.CompressNum(numGenPars, sig);
            MetaDataOut.CompressNum(numPars+numOptPars, sig);
            retType.TypeSig(sig);
            for(int i=0; i < numPars; i++) {
                parTypes[i].TypeSig(sig);
            }
            if(numOptPars > 0) {
                sig.WriteByte((byte)ElementType.Sentinel);
                for(int i=0; i < numOptPars; i++) {
                    optParTypes[i].TypeSig(sig);
                }
            }
        }

        internal string NameString()
        {
            string parString = "(";
            if(numPars > 0) {
                parString += parTypes[0].NameString();
                for(int i=1; i < numPars; i++) {
                    parString += "," + parTypes[i].NameString();
                }
            }
            if(numOptPars > 0) {
                if(numPars > 0)
                    parString += ",";
                parString += optParTypes[0].NameString();
                for(int i=1; i < numOptPars; i++) {
                    parString += "," + optParTypes[i].NameString();
                }
            }
            return name + parString;
        }

        internal void BuildTables(MetaDataOut md)
        {
            if(!retType.isDef())
                retType.BuildMDTables(md);
            for(int i=0; i < numPars; i++) {
                if(!parTypes[i].isDef())
                    parTypes[i].BuildMDTables(md);
            }
            for(int i=0; i < numOptPars; i++) {
                if(!optParTypes[i].isDef())
                    optParTypes[i].BuildMDTables(md);
            }
        }

        internal void BuildSignatures(MetaDataOut md)
        {
            if(!retType.isDef())
                retType.BuildSignatures(md);
            for(int i=0; i < numPars; i++) {
                if(!parTypes[i].isDef())
                    parTypes[i].BuildSignatures(md);
            }
            for(int i=0; i < numOptPars; i++) {
                if(!optParTypes[i].isDef())
                    optParTypes[i].BuildSignatures(md);
            }
        }

    }

    /**************************************************************************/
    /// <summary>
    /// Descriptor for a file containing a managed resource
    /// </summary>
    public class SourceFile {
        static ArrayList sourceFiles = new ArrayList();
        string name;
        Guid language, vendor, document;

        /*-------------------- Constructors ---------------------------------*/

        private SourceFile(string name, Guid lang, Guid vend, Guid docu)
        {
            this.name = name;
            language = lang;
            vendor = vend;
            document = docu;
            sourceFiles.Add(this);
        }

        private bool GuidsMatch(Guid lang, Guid vend, Guid docu)
        {
            if(language != lang)
                return false;
            if(vendor != vend)
                return false;
            if(document != docu)
                return false;
            return true;
        }

        internal bool Match(SourceFile file)
        {
            if(file == null)
                return false;
            if(this == file)
                return true;
            if(name != file.name)
                return false;
            return GuidsMatch(file.language, file.vendor, file.document);
        }

        public static SourceFile GetSourceFile(string name, Guid lang, Guid vend, Guid docu)
        {
            for(int i=0; i < sourceFiles.Count; i++) {
                SourceFile sFile = (SourceFile)sourceFiles[i];
                if((sFile.name == name) && sFile.GuidsMatch(lang, vend, docu))
                    return sFile;
            }
            return new SourceFile(name, lang, vend, docu);
        }

    }
    /**************************************************************************/
    // Descriptors for Native Types for parameter marshalling
    /**************************************************************************/
    /// <summary>
    /// Descriptors for native types used for marshalling
    /// </summary>
    public class NativeType {
        public static readonly NativeType Void = new NativeType(0x01);
        public static readonly NativeType Boolean = new NativeType(0x02);
        public static readonly NativeType Int8 = new NativeType(0x03);
        public static readonly NativeType UInt8 = new NativeType(0x04);
        public static readonly NativeType Int16 = new NativeType(0x05);
        public static readonly NativeType UInt16 = new NativeType(0x06);
        public static readonly NativeType Int32 = new NativeType(0x07);
        public static readonly NativeType UInt32 = new NativeType(0x08);
        public static readonly NativeType Int64 = new NativeType(0x09);
        public static readonly NativeType UInt64 = new NativeType(0x0A);
        public static readonly NativeType Float32 = new NativeType(0x0B);
        public static readonly NativeType Float64 = new NativeType(0x0C);
        public static readonly NativeType Currency = new NativeType(0x0F);
        public static readonly NativeType BStr = new NativeType(0x13);
        public static readonly NativeType LPStr = new NativeType(0x14);
        public static readonly NativeType LPWStr = new NativeType(0x15);
        public static readonly NativeType LPTStr = new NativeType(0x16);
        public static readonly NativeType FixedSysString = new NativeType(0x17);
        public static readonly NativeType IUnknown = new NativeType(0x19);
        public static readonly NativeType IDispatch = new NativeType(0x1A);
        public static readonly NativeType Struct = new NativeType(0x1B);
        public static readonly NativeType Interface = new NativeType(0x1C);
        public static readonly NativeType Int = new NativeType(0x1F);
        public static readonly NativeType UInt = new NativeType(0x20);
        public static readonly NativeType ByValStr = new NativeType(0x22);
        public static readonly NativeType AnsiBStr = new NativeType(0x23);
        public static readonly NativeType TBstr = new NativeType(0x24);
        public static readonly NativeType VariantBool = new NativeType(0x25);
        public static readonly NativeType FuncPtr = new NativeType(0x26);
        public static readonly NativeType AsAny = new NativeType(0x28);
        private static readonly NativeType[] nativeTypes = { null, Void, Boolean, Int8,  
                                                         UInt8, Int16, UInt16, Int32,
                                                         UInt32, Int64,  UInt64, 
                                                         Float32,  Float64, null, null,
                                                         Currency, null, null, null,
                                                         BStr, LPStr, LPWStr, LPTStr,
                                                         FixedSysString, null, IUnknown,
                                                         IDispatch, Struct, Interface,
                                                         null, null, Int, UInt, null,
                                                         ByValStr, AnsiBStr, TBstr,
                                                         VariantBool, FuncPtr, null,
                                                         AsAny};

        protected byte typeIndex;

        /*-------------------- Constructors ---------------------------------*/

        internal NativeType(byte tyIx)
        {
            typeIndex = tyIx;
        }

        internal byte GetTypeIndex()
        {
            return typeIndex;
        }

        internal static NativeType GetNativeType(int ix)
        {
            if(ix < nativeTypes.Length)
                return nativeTypes[ix];
            return null;
        }

        internal virtual byte[] ToBlob()
        {
            byte[] bytes = new byte[1];
            bytes[0] = GetTypeIndex();
            return bytes;
        }

    }

    /**************************************************************************/
    public class NativeArray : NativeType {
        NativeType elemType;
        uint len = 0, parNum = 0, elemMult = 1;
        internal static readonly byte ArrayTag = 0x2A;

        /*-------------------- Constructors ---------------------------------*/

        public NativeArray(NativeType elemType)
            : base((byte)NativeTypeIx.Array)
        {
            this.elemType = elemType;
        }

        public NativeArray(NativeType elemType, int len)
            : base((byte)NativeTypeIx.Array)
        {
            this.elemType = elemType;
            this.len = (uint)len;
        }

        public NativeArray(NativeType elemType, int numElem, int parNumForLen)
            : base((byte)NativeTypeIx.Array)
        {
            this.elemType = elemType;
            len = (uint)numElem;
            parNum = (uint)parNumForLen;
        }

        internal NativeArray(NativeType elemType, uint pNum, uint elemMult, uint numElem)
            : base((byte)NativeTypeIx.Array)
        {
            this.elemType = elemType;
            parNum = pNum;
            this.elemMult = elemMult;
            len = numElem;
        }

        internal override byte[] ToBlob()
        {
            MemoryStream str = new MemoryStream();
            str.WriteByte(GetTypeIndex());
            if(elemType == null)
                str.WriteByte(0x50);  // no info (MAX)
            else
                str.WriteByte(elemType.GetTypeIndex());
            MetaDataOut.CompressNum(parNum, str);
            MetaDataOut.CompressNum(elemMult, str);
            MetaDataOut.CompressNum(len, str);
            return str.ToArray();
        }

    }
    /**************************************************************************/
    internal class PEFileVersionInfo {

        internal bool fromExisting;
        internal ushort characteristics;
        internal bool isDLL;
        internal byte lMajor;
        internal byte lMinor;
        internal uint fileAlign;
        internal ushort osMajor;
        internal ushort osMinor;
        internal ushort userMajor;
        internal ushort userMinor;
        internal ushort subSysMajor;
        internal ushort subSysMinor;
        internal SubSystem subSystem;
        internal ushort DLLFlags = 0;
        internal ushort cliMajVer;
        internal ushort cliMinVer;
        internal CorFlags corFlags = CorFlags.CF_IL_ONLY;
        internal ushort mdMajVer;
        internal ushort mdMinVer;
        internal NetVersion netVersion;
        internal string netVerString;
        internal byte tsMajVer;
        internal byte tsMinVer;

        internal void SetDefaults(string name)
        {
            fromExisting = false;
            isDLL = name.EndsWith(".dll") || name.EndsWith(".DLL");
            if(isDLL) {
                characteristics = FileImage.dllCharacteristics;
            } else {
                characteristics = FileImage.exeCharacteristics;
            }
            lMajor = MetaData.LMajors[0];
            lMinor = 0;
            fileAlign = FileImage.minFileAlign;
            osMajor = 4;
            osMinor = 0;
            userMajor = 0;
            userMinor = 0;
            subSysMajor = 4;
            subSysMinor = 0;
            subSystem = SubSystem.Windows_CUI;
            DLLFlags = FileImage.DLLFlags;
            cliMajVer = 2;
            cliMinVer = 0;
            corFlags = CorFlags.CF_IL_ONLY;
            mdMajVer = 1;
            mdMinVer = 1; // MetaData Minor Version  ECMA = 0, PEFiles = 1
            netVersion = NetVersion.Everett;
            netVerString = MetaData.versions[0];
            tsMajVer = 1;
            tsMinVer = 0;
        }

    }

    /**************************************************************************/
    /**************************************************************************/
    /**************************************************************************/
    // Base Class for PE Files
    /**************************************************************************/
    /**************************************************************************/
    /**************************************************************************/
    /// <summary>
    /// Base class for the PEFile (starting point)
    /// </summary>
    public partial class PEFile : Module {
        private string outputDir, fileName, pdbFileName = null;
        private Stream outStream;
        private Assembly thisAssembly;
        PEWriter output;
        MetaDataOut metaData;
        System.IO.FileStream unmanagedResources;
        internal MetaDataTables metaDataTables;
        internal PEFileVersionInfo versionInfo;

        /*-------------------- Constructors ---------------------------------*/

        /// <summary>
        /// Create a new PE File with the name "fileName".  If "fileName" ends in ".dll" then
        /// the file is a dll, otherwise it is an exe file.  This PE File has no assembly.
        /// </summary>
        /// <param name="fileName">Name for the output file.</param>
        public PEFile(string fileName)
            : base(fileName)
        {
            PrimitiveType.ClearAddedFlags();   // KJG 18-April-2005
            this.fileName = fileName;
            metaData = new MetaDataOut();
            versionInfo = new PEFileVersionInfo();
            versionInfo.SetDefaults(fileName);
        }

        /// <summary>
        /// Create a new PE File with the name "fileName".  If "fileName" ends in ".dll" then
        /// the file is a dll, otherwise it is an exe file.  This file has an Assembly called
        /// "assemblyName".
        /// </summary>
        /// <param name="fileName">Name for the output file</param>
        /// <param name="assemblyName">Name of the assembly</param>
        public PEFile(string fileName, string assemblyName)
            : base(fileName)
        {
            PrimitiveType.ClearAddedFlags();   // KJG 18-April-2005
            this.fileName = fileName;
            thisAssembly = new Assembly(assemblyName, this);
            metaData = new MetaDataOut();
            versionInfo = new PEFileVersionInfo();
            versionInfo.SetDefaults(fileName);
        }

        /// <summary>
        /// Read a PE file and create all the data structures to represent it
        /// </summary>
        /// <param name="filename">The file name of the PE file</param>
        /// <returns>PEFile object representing "filename"</returns>
        public static PEFile ReadPEFile(string filename)
        {
            return PEReader.ReadPEFile(filename);
        }

        /// <summary>
        /// Read an existing PE File and return the exported interface 
        /// (ie. anything that was specified as public).  
        /// All the MetaData structures will be Refs.
        /// </summary>
        /// <param name="filename">The name of the pe file</param>
        /// <returns>The AssemblyRef or ModuleRef describing the exported interface of the specified file</returns>
        public static ReferenceScope ReadExportedInterface(string filename)
        {
            return PEReader.GetExportedInterface(filename);
        }

        public static PEFile ReadPublicClasses(string filename)
        {
            PEFile pefile = PEReader.ReadPEFile(filename);
            ArrayList newClasses = new ArrayList();
            for(int i = 0; i < pefile.classes.Count; i++) {
                ClassDef aClass = (ClassDef)pefile.classes[i];
                if(aClass.isPublic())
                    newClasses.Add(aClass);
            }
            pefile.classes = newClasses;
            return pefile;
        }

        /*---------------------------- public set and get methods ------------------------------*/

        /// <summary>
        /// Get the version of .NET for this PE file
        /// </summary>
        /// <returns>.NET version</returns>
        public NetVersion GetNetVersion()
        {
            return versionInfo.netVersion;
        }

        /// <summary>
        /// Set the .NET version for this PE file
        /// </summary>
        /// <param name="nVer">.NET version</param>
        public void SetNetVersion(NetVersion nVer)
        {
            versionInfo.netVersion = nVer;
            versionInfo.netVerString = MetaData.versions[(int)versionInfo.netVersion];
            if(nVer >= NetVersion.Whidbey40)
                versionInfo.tsMinVer = 1;
            GenericParam.extraField = nVer <= NetVersion.Whidbey40;
            if(Diag.DiagOn && GenericParam.extraField)
                Console.WriteLine("Writing extra field for GenericParams");
        }

        /// <summary>
        /// Get the .NET version for this PE file
        /// </summary>
        /// <returns>string representing the .NET version</returns>
        public string GetNetVersionString()
        {
            return versionInfo.netVerString.Trim();
        }

        /// <summary>
        /// Make a descriptor for an external assembly to this PEFile (.assembly extern)
        /// </summary>
        /// <param name="assemName">the external assembly name</param>
        /// <returns>a descriptor for this external assembly</returns>
        public AssemblyRef MakeExternAssembly(string assemName)
        {
            if(assemName.CompareTo(MSCorLib.mscorlib.Name()) == 0)
                return MSCorLib.mscorlib;
            return new AssemblyRef(assemName);
        }

        /// <summary>
        /// Make a descriptor for an external module to this PEFile (.module extern)
        /// </summary>
        /// <param name="name">the external module name</param>
        /// <returns>a descriptor for this external module</returns>
        public ModuleRef MakeExternModule(string name)
        {
            return new ModuleRef(name);
        }

        /// <summary>
        /// Set the directory that the PE File will be written to.  
        /// The default is the current directory.
        /// </summary>
        /// <param name="outputDir">The directory to write the PE File to.</param>
        public void SetOutputDirectory(string outputDir)
        {
            this.outputDir = outputDir;
        }

        /// <summary>
        /// Direct PE File output to an existing stream, instead of creating
        /// a new file.
        /// </summary>
        /// <param name="output">The output stream</param>
        public void SetOutputStream(Stream output)
        {
            this.outStream = output;
        }

        /// <summary>
        /// Specify if this PEFile is a .dll or .exe
        /// </summary>
        public void SetIsDLL(bool isDll)
        {
            versionInfo.isDLL = isDll;
            if(isDll)
                versionInfo.characteristics = FileImage.dllCharacteristics;
            else
                versionInfo.characteristics = FileImage.exeCharacteristics;
        }

        /// <summary>
        /// Set the subsystem (.subsystem) (Default is Windows Console mode)
        /// </summary>
        /// <param name="subS">subsystem value</param>
        public void SetSubSystem(SubSystem subS)
        {
            versionInfo.subSystem = subS;
        }

        /// <summary>
        /// Set the flags (.corflags)
        /// </summary>
        /// <param name="flags">the flags value</param>
        public void SetCorFlags(CorFlags flags)
        {
            versionInfo.corFlags = flags;
        }

        public string GetFileName()
        {
            return fileName;
        }

        public void SetFileName(string filename)
        {
            this.fileName = filename;
        }

        public void SetPDBFileName(string pdbName)
        {
            this.pdbFileName = pdbName;
        }

        /// <summary>
        /// Get a Meta Data Element from this PE file
        /// </summary>
        /// <param name="token">The meta data token for the required element</param>
        /// <returns>The meta data element denoted by token</returns>
        public MetaDataElement GetElement(uint token)
        {
            if(buffer != null)
                return buffer.GetTokenElement(token);
            if(metaDataTables != null)
                return metaDataTables.GetTokenElement(token);
            return null;
        }

        /// <summary>
        /// Add a manifest resource to this PEFile 
        /// </summary>
        public void AddUnmanagedResources(string resFilename)
        {
            if(!System.IO.File.Exists(resFilename))
                throw (new FileNotFoundException("Unmanaged Resource File Not Found", resFilename));
            unmanagedResources = System.IO.File.OpenRead(resFilename);
            throw new NotYetImplementedException("Unmanaged Resources are not yet implemented");
        }

        /// <summary>
        /// Add a managed resource to this PEFile.  The resource will be embedded in this PE file. 
        /// </summary>
        /// <param name="resName">The name of the managed resource</param>
        /// <param name="resBytes">The managed resource</param>
        /// <param name="isPublic">Access for the resource</param>
        public void AddManagedResource(string resName, byte[] resBytes, bool isPublic)
        {
            resources.Add(new ManifestResource(this, resName, resBytes, isPublic));
        }

        /// <summary>
        /// Add a managed resource from another assembly.
        /// </summary>
        /// <param name="resName">The name of the resource</param>
        /// <param name="assem">The assembly where the resource is</param>
        /// <param name="isPublic">Access for the resource</param>
        public void AddExternalManagedResource(string resName, AssemblyRef assem, bool isPublic)
        {
            resources.Add(new ManifestResource(this, resName, assem, 0, isPublic));
        }

        /// <summary>
        /// Add a managed resource from another file in this assembly.
        /// </summary>
        /// <param name="resName">The name of the resource</param>
        /// <param name="assem">The assembly where the resource is</param>
        /// <param name="isPublic">Access for the resource</param>
        public void AddExternalManagedResource(string resName, ResourceFile resFile, uint offset, bool isPublic)
        {
            resources.Add(new ManifestResource(this, resName, resFile, offset, isPublic));
        }

        /// <summary>
        /// Add a managed resource from another module in this assembly.
        /// </summary>
        /// <param name="resName">The name of the resource</param>
        /// <param name="assem">The assembly where the resource is</param>
        /// <param name="isPublic">Access for the resource</param>
        public void AddExternalManagedResource(string resName, ModuleRef mod, uint offset, bool isPublic)
        {
            resources.Add(new ManifestResource(this, resName, mod.modFile, offset, isPublic));
        }

        /// <summary>
        /// Add a managed resource from another assembly.
        /// </summary>
        /// <param name="mr"></param>
        /// <param name="isPublic"></param>
        public void AddExternalManagedResource(ManifestResource mr, bool isPublic)
        {
            resources.Add(new ManifestResource(this, mr, isPublic));
        }

        /// <summary>
        /// Find a resource
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <returns>The resource with the name "name" or null </returns>
        public ManifestResource GetResource(string name)
        {
            for(int i = 0; i < resources.Count; i++) {
                if(((ManifestResource)resources[i]).Name == name)
                    return (ManifestResource)resources[i];
            }
            return null;
        }

        public ManifestResource[] GetResources()
        {
            return (ManifestResource[])resources.ToArray(typeof(ManifestResource));
        }

        /// <summary>
        /// Get the descriptor for this assembly.  The PEFile must have been
        /// created with hasAssembly = true
        /// </summary>
        /// <returns>the descriptor for this assembly</returns>
        public Assembly GetThisAssembly()
        {
            return thisAssembly;
        }

        public AssemblyRef[] GetImportedAssemblies()
        {
            return buffer.GetAssemblyRefs();
        }

        public string[] GetNamesOfImports()
        {
            return buffer.GetAssemblyRefNames();
        }

        /*------------------------------------------ Output Methods -------------------------------*/

        /// <summary>
        /// Write out the PEFile (the "bake" function)
        /// </summary>
        public void WritePEFile(bool debug)
        {
            if(outStream == null) {
                if(outputDir != null) {
                    if(!outputDir.EndsWith("\\"))
                        fileName = outputDir + "\\" + fileName;
                    else
                        fileName = outputDir + fileName;
                }
                output = new PEWriter(versionInfo, fileName, metaData, debug);
            } else
                output = new PEWriter(versionInfo, outStream, metaData, debug);
            //output.SetPDBFile(pdbFileName);
            BuildMDTables(metaData);
            if(thisAssembly != null) {
                thisAssembly.BuildMDTables(metaData);
            }
            metaData.BuildMDTables(); // DoCustomAttributes, BuildSignatures for each in metadata tables
            output.MakeFile(versionInfo);
        }

        internal void SetThisAssembly(Assembly assem)
        {
            if(Diag.DiagOn)
                Console.WriteLine("Setting fileScope to assembly " + assem.Name());
            thisAssembly = assem;
        }

        internal void AddToResourceList(ManifestResource res)
        {
            resources.Add(res);
        }

        internal void AddToFileList()
        {
        }

        internal void SetDLLFlags(ushort dflags)
        {
            versionInfo.DLLFlags = dflags;
        }
    }

    /*
     *  public void ReadPDB() {
     *    PERWAPI.PDB.PDBReader reader = PERWAPI.PDB.Impl.ObtainReader(this, fileName);
     *    foreach (ClassDef cDef in GetClasses())
     *      foreach (MethodDef mDef in cDef.GetMethods()) {
     *        CILInstructions buffer = mDef.GetCodeBuffer();
     *        PERWAPI.PDB.Method meth = reader.GetMethod((int) mDef.Token());
     *
     *        if (meth == null)
     *          continue; // no symbols for this method
     *
     *        PERWAPI.PDB.SequencePoint[] spList = meth.SequencePoints;
     *
     *        MergeBuffer mergeBuffer = new MergeBuffer(buffer.GetInstructions());
     *
     *        PERWAPI.PDB.Scope outer = meth.Scope;
     *        // attempt to drop outer / useless scope
     *        if (meth.Scope.Variables.Length == 0 && meth.Scope.Children.Length == 1)
     *          outer = outer.Children[0];
     *        
     *        buffer.currentScope = ReadPDBScope(outer, mergeBuffer, null, mDef);
     *
     *        foreach (PERWAPI.PDB.SequencePoint sp in spList) {
     *          PERWAPI.PDB.Document doc = sp.Document;
     *          mergeBuffer.Add(new Line((uint) sp.Line, (uint) sp.Column, (uint) sp.EndLine, (uint) sp.EndColumn,
     *            SourceFile.GetSourceFile(doc.URL, doc.Language, doc.LanguageVendor, doc.DocumentType)),
     *            (uint) sp.Offset);
     *        }
     *
     *        buffer.SetInstructions(mergeBuffer.Instructions);
     *      }
     *   }
     *
     *   private Scope ReadPDBScope(PERWAPI.PDB.Scope scope, MergeBuffer mergeBuffer, Scope parent, MethodDef thisMeth) {
     *    Scope thisScope = new Scope(parent, thisMeth);
     *
     *    if (parent != null) mergeBuffer.Add(new OpenScope(thisScope), (uint) scope.StartOffset);
     *
     *    foreach (PERWAPI.PDB.Variable var in scope.Variables)
     *      thisScope.AddLocalBinding(var.Name, var.Address);
     *
     *    foreach (PERWAPI.PDB.Scope child in scope.Children)
     *      ReadPDBScope(child, mergeBuffer, thisScope, thisMeth);
     *
     *    if (parent != null) mergeBuffer.Add(new CloseScope(thisScope), (uint) scope.EndOffset);
     *
     *    return thisScope;
     *  }
     *}
     *
     *public abstract class PEResourceElement {
     *
     *  private int id;
     *  private string name;
     *
     *  public PEResourceElement() {}
     *
     *  public int Id { 
     *    get { return id; } 
     *    set { id = value; }
     *  }
     *
     *  public string Name { 
     *    get { return name; } 
     *    set { name = value; }
     *  }
     *
     *  protected internal abstract uint Size();
     *
     *  protected internal abstract void Write(BinaryWriter dest, uint RVA);
     *}
     *
     *public class PEResourceDirectory : PEResourceElement {
     *  public PEResourceDirectory() { }
     *
     *  private uint date = 0;
     *  private ushort majver = 1;
     *  private ushort minver = 0;
     *
     *  public uint Date { get { return date; } set { date = value; }}
     *  public ushort MajVer { get { return majver; } set { majver = value; }}
     *  public ushort MinVer { get { return minver; } set { minver = value; }}
     *
     *  private ArrayList subitems = new ArrayList();
     *
     *  public PEResourceElement this[int i] { get { return (PEResourceElement)subitems[i]; } }
     *
     *  public int Count() { return subitems.Count; }
     *
     *  public bool HasData()
     *  {
     *    return subitems.Count > 0;
     *  }
     *
     *  public void AddElement(PEResourceElement el)
     *  {
     *    subitems.Add(el);
     *  }
     *  private uint subsize, namesize, dirsize, numnamed;
     *  protected internal override uint Size()
     *  {
     *    namesize = 0;
     *    numnamed = 0;
     *    subsize = 0;
     *    for (int i = 0; i < subitems.Count; i++)
     *    {
     *      PEResourceElement el = (PEResourceElement)subitems[i];
     *      subsize += el.Size();
     *      if (el.Name != null)
     *      {
     *        namesize += 2 + (uint)el.Name.Length * 2;
     *        numnamed ++;
     *      }
     *    }
     *    dirsize = (uint)subitems.Count * 8 + 16;
     *    return dirsize + namesize + subsize;
     *  }
     *
     *  protected internal override void Write(BinaryWriter dest, uint RVA)
     *  {
     *    Size();
     *    dest.Flush();
     *    uint startnameoffset = (uint)dest.BaseStream.Position + (uint)dirsize;
     *    uint curritemoffset = startnameoffset + (uint)namesize;
     *    dest.Write((uint)0); // characteristics
     *    dest.Write(date); // datetime
     *    dest.Write(majver);
     *    dest.Write(minver);
     *    dest.Write((ushort)numnamed);
     *    dest.Write((ushort)(subitems.Count - numnamed));
     *
     *    uint currnameoffset = startnameoffset;
     *    for (int i = 0; i < subitems.Count; i++)
     *    {
     *      PEResourceElement el = (PEResourceElement)subitems[i];
     *      if (el.Name != null)
     *      {
     *        dest.Write((uint)(currnameoffset | 0x80000000));
     *        if (el is PEResourceDirectory)
     *          dest.Write((uint)(curritemoffset | 0x80000000));
     *        else
     *          dest.Write((uint)curritemoffset);
     *        currnameoffset += 2 + (uint)el.Name.Length * 2;
     *      }
     *      curritemoffset += el.Size();
     *    }
     *    curritemoffset = startnameoffset + namesize;
     *    for (int i = 0; i < subitems.Count; i++)
     *    {
     *      PEResourceElement el = (PEResourceElement)subitems[i];
     *      if (el.Name == null)
     *      {
     *        dest.Write(el.Id);
     *        if (el is PEResourceDirectory)
     *          dest.Write((uint)(curritemoffset | 0x80000000));
     *        else
     *          dest.Write((uint)curritemoffset);
     *      }
     *      curritemoffset += el.Size();
     *    }
     *    for (int i = 0; i < subitems.Count; i++)
     *    {
     *      PEResourceElement el = (PEResourceElement)subitems[i];
     *      string s = el.Name;
     *      if (s != null)
     *      {
     *        dest.Write((ushort)s.Length);
     *        byte[] b = System.Text.Encoding.Unicode.GetBytes(s);
     *        dest.Write(b);
     *      }
     *    }
     *    for (int i = 0; i < subitems.Count; i++)
     *    {
     *      PEResourceElement el = (PEResourceElement)subitems[i];
     *      el.Write(dest, RVA);
     *    }
     *
     *  }
     *}
     *
     *public class PEResourceData : PEResourceElement
     *{
     *  public PEResourceData() {}
     *  int codepage = 0;
     *  byte[] data;
     *
     *  public int CodePage { get { return codepage; } set { codepage = value; }}
     *
     *  public byte[] Data { get { return data; } set { data = value; }}
     *
     *  protected internal override uint Size()
     *  {
     *    return 16 + (uint)Data.Length;
     *  }
     *
     *  protected internal override void Write(BinaryWriter dest, uint RVA)
     *  {
     *    dest.Flush();
     *    dest.Write((uint)(dest.BaseStream.Position + 16) + RVA);
     *    dest.Write((uint)data.Length);
     *    dest.Write((uint)codepage);
     *    dest.Write((uint)0);
     *    dest.Write(data);
     *  }
     *}
    */
    /**************************************************************************/
    /*
    // Added to enable PDB reading
     * 
     *  internal class MergeBuffer {
     *    private CILInstruction[] _buffer;
     *    private ArrayList _debugBuffer;
     *    private int _current;
     *
     *    public MergeBuffer(CILInstruction[] buffer) {
     *      _debugBuffer = new ArrayList();
     *      _buffer = buffer;
     *    }
     *
     *    public void Add(CILInstruction inst, uint offset) {
     *      while (_current < _buffer.Length && _buffer[_current].offset < offset)
     *        _debugBuffer.Add(_buffer[_current++]);
     *      if (_debugBuffer.Count > 0 && offset >= ((CILInstruction) _debugBuffer[_debugBuffer.Count - 1]).offset) {
     *        inst.offset = offset;
     *        _debugBuffer.Add(inst);
     *      } else {
     *        int i;
     *
     *        for (i = 0; i < _debugBuffer.Count; i++)
     *          if (((CILInstruction) _debugBuffer[i]).offset > offset)
     *            break;
     *
     *        inst.offset = offset;
     *        _debugBuffer.Insert((i > 0 ? i - 1 : i), inst);
     *      }
     *    }
     *
     *    public CILInstruction[] Instructions {
     *      get { 
     *        while (_current < _buffer.Length)
     *          _debugBuffer.Add(_buffer[_current++]);
     *        return (CILInstruction[]) _debugBuffer.ToArray(typeof(CILInstruction)); 
     *      }
     *    }
     *  }
     */
    /**************************************************************************/
}
