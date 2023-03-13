#pragma once


struct IObject {
protected:
	virtual ~IObject();
public:
	virtual void Preserve() = 0;
	virtual void Release() = 0;
	virtual IObject* QueryInterface(const IID& iid) = 0;
};
