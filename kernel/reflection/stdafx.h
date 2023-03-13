#pragma once

#include "../stdtype.h"
#include "../stdlib.h"
#include "../stl.h"
#include "../metadata.h"
#include "../signature.h"


#define interface /*__declspec(novtable)*/ struct

namespace Reflection {

	enum DataType {
		DT_i1,
		DT_i2,
		DT_i4,
		DT_i8,
		DT_u1,
		DT_u2,
		DT_u4,
		DT_u8,
		DT_i,
		DT_u,
		DT_o,
		DT_f4,
		DT_f8,
		DT_f,
		DT_p,
	};

	struct Variable {
		DataType type;
		union {
			char	i1;
			short	i2;
			int32	i4;
			//int64	i8;
			byte	u1;
			ushort	u2;
			uint32	u4;
			//uint64	u8;
			int		i;
			uint	u;
			void*	p;
			byte*	o;
		};
		Variable() : type(DT_i4), i(0) {
		}
		Variable(DataType dt, int val) : type(dt), i(val) {
		}
	};

	class ReflectionItem {
		const Metadata::MetadataRoot& root;
	protected:
		ReflectionItem(const Metadata::MetadataRoot& _root) : root(_root) {
		}
	public:
		const Metadata::MetadataRoot& getRoot() const { return root; }
	};

	/*
	interface IMemberRefChild {
		virtual TableId getTableId() const = 0;
	};

	interface IMemberRefParent {
		virtual std::string getFullName() const = 0;
		virtual IMemberRefChild* Resolve(const char* name, uint signature) const = 0;
	};

	interface IResolutionScope {
	};
	*/

}
