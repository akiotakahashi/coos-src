using System;
using System.IO;
using System.Collections.Generic;

namespace CooS.Formats.Java.Description {

	public class FieldSig {

		private BasicTypes elemtype;
		private string name;
		private FieldSig comp;

		public FieldSig(string text) {
			Reader reader = new Reader(text);
			this.FieldDescriptor(reader);
		}

		public FieldSig() {
		}

		public override string ToString() {
			if(this.IsBaseType) {
				switch(this.elemtype) {
				case BasicTypes.Boolean:
					return "boolean";
				case BasicTypes.Char:
					return "char";
				case BasicTypes.I1:
					return "byte";
				case BasicTypes.I2:
					return "short";
				case BasicTypes.I4:
					return "int";
				case BasicTypes.I8:
					return "long";
				case BasicTypes.R4:
					return "float";
				case BasicTypes.R8:
					return "double";
				case BasicTypes.Void:
					return "void";
				default:
					throw new UnexpectedException();
				}
			} else if(this.IsArrayType) {
				return this.comp.ToString()+"[]";
			} else {
				return this.name.Replace('/','.');
			}
		}

		public string ToDescription() {
			if(this.IsBaseType) {
				return this.elemtype.ToString();
			} else if(this.IsArrayType) {
				return "["+this.comp.ToDescription();
			} else {
				return "L"+this.name+";";
			}
		}

		public void FieldDescriptor(Reader reader) {
			FieldType(reader);
		}

		public void ComponentType(Reader reader) {
			FieldType(reader);
		}

		public void FieldType(Reader reader) {
			// BaseType
			//		B
			//		C
			//		D
			//		F
			//		I
			//		J
			//		S
			//		Z
			// ObjectType
			//		L <classname> ;
			// ArrayType
			//		[ ComponentType
			switch(reader.Read()) {
			case 'B':
				elemtype = BasicTypes.I1;
				break;
			case 'C':
				elemtype = BasicTypes.Char;
				break;
			case 'D':
				elemtype = BasicTypes.R8;
				break;
			case 'F':
				elemtype = BasicTypes.R4;
				break;
			case 'I':
				elemtype = BasicTypes.I4;
				break;
			case 'J':
				elemtype = BasicTypes.I8;
				break;
			case 'S':
				elemtype = BasicTypes.I2;
				break;
			case 'Z':
				elemtype = BasicTypes.Boolean;
				break;
			case 'L':
				this.name = reader.Read(';');
				break;
			case '[':
				this.comp = new FieldSig();
				this.comp.ComponentType(reader);
				break;
			default:
				throw new BadImageException();
			}
		}

		public bool IsBaseType {
			get {
				return this.name==null && this.comp==null;
			}
		}

		public bool IsObjectType {
			get {
				return this.name!=null;
			}
		}

		public bool IsArrayType {
			get {
				return this.comp!=null;
			}
		}

		public BasicTypes BasicType {
			get {
				return this.elemtype;
			}
		}

		public string TypeName {
			get {
				return this.name;
			}
		}

		public FieldSig ElementType {
			get {
				return this.comp;
			}
		}

	}
	
}
