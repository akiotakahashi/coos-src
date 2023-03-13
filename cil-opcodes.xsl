<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">
	<xsl:template match="opdesc">
		<xsl:apply-templates>
			<xsl:sort select="@name" />
		</xsl:apply-templates>
	</xsl:template>
	<xsl:template match="opcode">
		case "<xsl:value-of select="@name" />":<br />
		break;<br />
	</xsl:template>
</xsl:stylesheet>
