package inc.awesome.metathrift;

import inc.awesome.metathrift.Meta;

import org.junit.Test;

import static org.junit.Assert.*;
import static inc.awesome.metathrift.Meta.*;

public class CustomTypesTest {

	@Test
	public void Assert_registerType() {
		final String someTypeName = "de.schoenhofer.SomeType";
		registerType(SomeType.class, someTypeName);
		
        SomeType expected = new SomeType();
        expected.setUri("http://www.heise.de");
        
        MetaObject mo = toMetaObject(expected, SomeType.class);
        SomeType actual = (SomeType)fromMetaObject(mo);
        assertEquals(expected, actual);
        assertEquals(mo.getTypeName(), someTypeName);

        Meta.unregisterType(SomeType.class);

        mo = Meta.toMetaObject(expected, SomeType.class);
        assertEquals(SomeType.class.getName(), mo.getTypeName());
	}
}

class SomeType
{
	private String uri;
	
	public String getUri() { return uri; }
	public void setUri(String uri) { this.uri = uri; }
	
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((uri == null) ? 0 : uri.hashCode());
		return result;
	}
	
	@Override
	public boolean equals(Object obj) {
		if (this == obj) return true;
		if (obj == null) return false;
		if (getClass() != obj.getClass()) return false;
		SomeType other = (SomeType) obj;
		if (uri == null) {
			if (other.uri != null) return false;
		} else if (!uri.equals(other.uri))
			return false;
		return true;
	}
}

