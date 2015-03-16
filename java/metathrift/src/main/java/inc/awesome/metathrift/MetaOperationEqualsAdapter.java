package inc.awesome.metathrift;

import org.apache.commons.lang3.builder.HashCodeBuilder;

public class MetaOperationEqualsAdapter 
{
	final MetaOperation operation;
	public MetaOperation getOperation() { return operation; }
	public MetaOperationEqualsAdapter(MetaOperation func) { this.operation = func; }
	
	@Override
	public int hashCode() {
	     return new HashCodeBuilder(19, 41).
	       append(operation.getName()).
	       append(operation.getInputTypeName()).
	       append(operation.getOutputTypeName()).
	       toHashCode();		
	}
	
	@Override
	public boolean equals(Object obj) {
		if (this == obj) return true;
		if (obj == null || !(obj instanceof MetaOperationEqualsAdapter)) return false;
		MetaOperationEqualsAdapter that = (MetaOperationEqualsAdapter)obj;
		return Meta.safeEquals(this.getOperation(), that.getOperation());
	}
}
