using System;
using System.IO;
using System.Collections.Generic;
using CooS.Reflection;
using CooS.Execution;

namespace CooS.Interpret {

	public struct Block {

		public TypeInfo Type;
		public byte[] Data;

		public override string ToString() {
			StringWriter writer = new StringWriter();
			writer.Write(this.Type.FullName);
			writer.Write(" (");
			for(int i=this.Data.Length-1; i>=0; --i) {
				writer.Write("{0:X2}", this.Data[i]);
			}
			writer.Write(")");
			return writer.ToString();
		}

		public static unsafe explicit operator int(Block block) {
			if(block.Type.IsSigned) {
				switch(block.Type.IntrinsicType) {
				case IntrinsicTypes.Fp32:
					fixed(byte* p = block.Data) {
						return (int)*(float*)p;
					}
				case IntrinsicTypes.Fp64:
					fixed(byte* p = block.Data) {
						return (int)*(double*)p;
					}
				default:
					switch(block.Type.IntrinsicSize) {
					case 1:
						return (int)(sbyte)block.Data[0];
					case 2:
						fixed(byte* p = block.Data) {
							return (int)*(short*)p;
						}
					case 4:
						fixed(byte* p = block.Data) {
							return (int)*(int*)p;
						}
					case 8:
						fixed(byte* p = block.Data) {
							return (int)*(long*)p;
						}
					default:
						throw new NotSupportedException();
					}
				}
			} else {
				switch(block.Type.IntrinsicSize) {
				case 1:
					return (int)(byte)block.Data[0];
				case 2:
					fixed(byte* p = block.Data) {
						return (int)*(ushort*)p;
					}
				case 4:
					fixed(byte* p = block.Data) {
						return (int)*(uint*)p;
					}
				case 8:
					fixed(byte* p = block.Data) {
						return (int)*(ulong*)p;
					}
				default:
					throw new NotSupportedException();
				}
			}
		}

		public static unsafe explicit operator long(Block block) {
			if(block.Type.IsSigned) {
				switch(block.Type.IntrinsicType) {
				case IntrinsicTypes.Fp32:
					fixed(byte* p = block.Data) {
						return (long)*(float*)p;
					}
				case IntrinsicTypes.Fp64:
					fixed(byte* p = block.Data) {
						return (long)*(double*)p;
					}
				default:
					switch(block.Type.IntrinsicSize) {
					case 1:
						return (long)(sbyte)block.Data[0];
					case 2:
						fixed(byte* p = block.Data) {
							return (long)*(short*)p;
						}
					case 4:
						fixed(byte* p = block.Data) {
							return (long)*(int*)p;
						}
					case 8:
						fixed(byte* p = block.Data) {
							return (long)*(long*)p;
						}
					default:
						throw new NotSupportedException();
					}
				}
			} else {
				switch(block.Type.IntrinsicSize) {
				case 1:
					return (long)(byte)block.Data[0];
				case 2:
					fixed(byte* p = block.Data) {
						return (long)*(ushort*)p;
					}
				case 4:
					fixed(byte* p = block.Data) {
						return (long)*(uint*)p;
					}
				case 8:
					fixed(byte* p = block.Data) {
						return (long)*(ulong*)p;
					}
				default:
					throw new NotSupportedException();
				}
			}
		}

		public static unsafe explicit operator float(Block block) {
			if(block.Type.IsSigned) {
				switch(block.Type.IntrinsicType) {
				case IntrinsicTypes.Fp32:
					fixed(byte* p = block.Data) {
						return (float)*(float*)p;
					}
				case IntrinsicTypes.Fp64:
					fixed(byte* p = block.Data) {
						return (float)*(double*)p;
					}
				default:
					switch(block.Type.IntrinsicSize) {
					case 1:
						return (float)(sbyte)block.Data[0];
					case 2:
						fixed(byte* p = block.Data) {
							return (float)*(short*)p;
						}
					case 4:
						fixed(byte* p = block.Data) {
							return (float)*(int*)p;
						}
					case 8:
						fixed(byte* p = block.Data) {
							return (float)*(long*)p;
						}
					default:
						throw new NotSupportedException();
					}
				}
			} else {
				switch(block.Type.IntrinsicSize) {
				case 1:
					return (float)(byte)block.Data[0];
				case 2:
					fixed(byte* p = block.Data) {
						return (float)*(ushort*)p;
					}
				case 4:
					fixed(byte* p = block.Data) {
						return (float)*(uint*)p;
					}
				case 8:
					fixed(byte* p = block.Data) {
						return (float)*(ulong*)p;
					}
				default:
					throw new NotSupportedException();
				}
			}
		}

		public static unsafe explicit operator double(Block block) {
			if(block.Type.IsSigned) {
				switch(block.Type.IntrinsicType) {
				case IntrinsicTypes.Fp32:
					fixed(byte* p = block.Data) {
						return (double)*(float*)p;
					}
				case IntrinsicTypes.Fp64:
					fixed(byte* p = block.Data) {
						return (double)*(double*)p;
					}
				default:
					switch(block.Type.IntrinsicSize) {
					case 1:
						return (double)(sbyte)block.Data[0];
					case 2:
						fixed(byte* p = block.Data) {
							return (double)*(short*)p;
						}
					case 4:
						fixed(byte* p = block.Data) {
							return (double)*(int*)p;
						}
					case 8:
						fixed(byte* p = block.Data) {
							return (double)*(long*)p;
						}
					default:
						throw new NotSupportedException();
					}
				}
			} else {
				switch(block.Type.IntrinsicSize) {
				case 1:
					return (double)(byte)block.Data[0];
				case 2:
					fixed(byte* p = block.Data) {
						return (double)*(ushort*)p;
					}
				case 4:
					fixed(byte* p = block.Data) {
						return (double)*(uint*)p;
					}
				case 8:
					fixed(byte* p = block.Data) {
						return (double)*(ulong*)p;
					}
				default:
					throw new NotSupportedException();
				}
			}
		}

		public static unsafe explicit operator Address(Block block) {
			if(block.Data.Length!=Architecture.Target.AddressSize) {
				throw new ArgumentException();
			} else {
				return new Address(BitConverter.ToInt32(block.Data,0), block.Type);
			}
		}

	}

}
