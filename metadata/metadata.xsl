<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">
	<xsl:template match="/*">
		<xsl:apply-templates select="table[not(@type='stream')]">
			<xsl:sort select="@name"/>
		</xsl:apply-templates>
		<xsl:apply-templates select="index">
			<xsl:sort select="@name"/>
		</xsl:apply-templates>
	</xsl:template>
	<xsl:template name="typename">
		<xsl:param name="type"/>
		<xsl:choose>
			<xsl:when test="count(../../flags[@name=$type])&gt;0">
				<xsl:value-of select="../../flags/@name[../@name=$type]"/>
			</xsl:when>
			<xsl:when test="count(../../table[@name=$type])&gt;0">uint</xsl:when>
			<xsl:when test="count(../../index[@name=$type])&gt;0"><xsl:value-of select="../../index/@name[../@name=$type]"/>Index</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$type"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="bitsize">
		<xsl:param name="count"></xsl:param>
		<xsl:choose>
			<xsl:when test="$count>128">8</xsl:when>
			<xsl:when test="$count>64">7</xsl:when>
			<xsl:when test="$count>32">6</xsl:when>
			<xsl:when test="$count>16">5</xsl:when>
			<xsl:when test="$count>8">4</xsl:when>
			<xsl:when test="$count>4">3</xsl:when>
			<xsl:when test="$count>2">2</xsl:when>
			<xsl:when test="$count>1">1</xsl:when>
			<xsl:otherwise>1 /* ? */</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!--
	<xsl:template match="table">
		case TABLE_<xsl:value-of select="@name"/>:<br/>
		return new <xsl:value-of select="@name"/>(row, idxsizes);<br/>
	</xsl:template>
	-->
	<!--
	<xsl:template match="flags"> enum <xsl:value-of select="@name"/> {<br/>
		<xsl:apply-templates select="value"/> }; <br/><br/>
	</xsl:template>
	<xsl:template match="value">
		<xsl:value-of select="@name"/> = <xsl:value-of select="@value"></xsl:value-of>,
		<xsl:if test="string-length(.)>0">// <xsl:value-of select="."></xsl:value-of></xsl:if>
		<br/>
	</xsl:template>
	-->
	<xsl:template match="index">
		class <xsl:value-of select="@name"/>Index : public Index {<br/>
		static TableId tables[];<br/>
		public:<br/>
		static uint CalcSize(const uint rowcounts[]) {<br/>
			return getSize(tables, <xsl:value-of select="count(tableref)"></xsl:value-of>, rowcounts);<br/>
		}<br/>
		public:<br/>
		<xsl:value-of select="@name"/>Index() {<br/>
		}<br/>
		<xsl:value-of select="@name"/>Index(uint value) {<br/>
			parse(value, tables, <xsl:value-of select="count(tableref)"></xsl:value-of>);<br/>
		} <br/>
		};<br/>
		<br/>
	</xsl:template>
	<!--
	<xsl:template match="index">
		TableId <xsl:value-of select="@name"/>Index::tables[] = {<br/>
		<xsl:apply-templates select="tableref"/>
		};<br/>
		<br/>
	</xsl:template>
	-->
	<xsl:template match="tableref">
		TABLE_<xsl:value-of select="."></xsl:value-of>,<br/>
	</xsl:template>
	<!--
	<xsl:template match="tableref">
		case <xsl:value-of select="position()-1"></xsl:value-of>:<br/>
		table = TABLE_<xsl:value-of select="."></xsl:value-of>;<br/>
		break;<br/>
	</xsl:template>
	<xsl:template match="table"> struct <xsl:value-of select="@name"/> : Table {<br/>
		<xsl:apply-templates select="field"/>
		<xsl:value-of select="@name"/>(const byte* row, const uint idxsizes[]) {<br/>
		<xsl:apply-templates select="field" mode="parse"/> } <br/> }; <br/>
		<br/>
	</xsl:template>
	<xsl:template match="field">
		<xsl:variable name="type" select="@type"/>
		<xsl:call-template name="typename">
			<xsl:with-param name="type">
				<xsl:value-of select="$type"/>
			</xsl:with-param>
		</xsl:call-template>
		<xsl:text> </xsl:text>
		<xsl:value-of select="@name"/>;<br/>
	</xsl:template>
	<xsl:template match="field" mode="parse">
		<xsl:variable name="type" select="@type"/>
		<xsl:variable name="fetch">
			Fetch(row, 
			<xsl:choose>
				<xsl:when test="count(../../flags[@name=$type])&gt;0">
					<xsl:value-of select="../../flags/@size[../@name=$type]"/>
				</xsl:when>
				<xsl:when test="count(../../table[@name=$type])&gt;0"> idxsizes[INDEX_<xsl:value-of
					select="../../table/@name[../@name=$type]"/>] </xsl:when>
				<xsl:when test="count(../../index[@name=$type])&gt;0"> idxsizes[INDEX_<xsl:value-of
					select="../../index/@name[../@name=$type]"/>] </xsl:when>
				<xsl:otherwise> sizeof(<xsl:value-of select="$type"/>) </xsl:otherwise>
			</xsl:choose>
			)
		</xsl:variable>
		<xsl:value-of select="@name"/> =
		<xsl:choose>
			<xsl:when test="count(../../index[@name=$type])&gt;0">
				<xsl:value-of select="../../index/@name[../@name=$type]"/>Index(<xsl:value-of select="$fetch"></xsl:value-of>)
			</xsl:when>
			<xsl:otherwise>
				(<xsl:call-template name="typename">
					<xsl:with-param name="type">
						<xsl:value-of select="$type"/>
					</xsl:with-param>
				</xsl:call-template>)<xsl:value-of select="$fetch"></xsl:value-of>
			</xsl:otherwise>
		</xsl:choose>
		;<br/>
	</xsl:template>
	-->
	<!--
	<xsl:template match="table">
		TABLE_<xsl:value-of select="@name"/> = 0x<xsl:value-of select="@id"/>,<br/>
	</xsl:template>
	-->
	<!--
	<xsl:template match="table">
		case TABLE_<xsl:value-of select="@name"/>:<br/>
		return idxsz.sz<xsl:value-of select="@name"/>;<br/>
	</xsl:template>
	-->
	<!--
	<xsl:template match="table">
		idxsizes[INDEX_<xsl:value-of select="@name"/>] = getIndexFieldSize(getRowCount(INDEX_<xsl:apply-templates select="@name"/>));<br/>
	</xsl:template>
	<xsl:template match="index">
		idxsizes[INDEX_<xsl:value-of select="@name"/>] = <xsl:value-of select="@name"/>Index::CalcSize(rowcounts);<br/>
	</xsl:template>
	<xsl:template match="flags"></xsl:template>
	-->
	<!--
	<xsl:template match="table">
		rowsizes[TABLE_<xsl:value-of select="@name"/>] = <xsl:apply-templates select="field"/>;<br/>
	</xsl:template>
	<xsl:template match="field">
		<xsl:variable name="type" select="@type" />
		+
		<xsl:choose>
		<xsl:when test="count(../../flags[@name=$type])>0">
			<xsl:value-of select="../../flags/@size[../@name=$type]"/>
			/* <xsl:value-of select="../../flags/@name[../@name=$type]"/> */
		</xsl:when>
		<xsl:when test="count(../../table[@name=$type])>0">
			idxsizes[INDEX_<xsl:value-of select="../../table/@name[../@name=$type]"/>]
		</xsl:when>
		<xsl:when test="count(../../index[@name=$type])>0">
			idxsizes[INDEX_<xsl:value-of select="../../index/@name[../@name=$type]"/>]
		</xsl:when>
		<xsl:otherwise>
			sizeof(<xsl:value-of select="@type"/>)
		</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	-->
	<!--
	<xsl:template match="table">
		INDEX_<xsl:value-of select="@name"/>,<br/>
	</xsl:template>
	<xsl:template match="index">
		INDEX_<xsl:value-of select="@name"/>,<br/>
	</xsl:template>
	-->
	<!--
	<xsl:template match="table">
		case INDEX_<xsl:value-of select="@name"/>:<br/>
		return rows[TABLE_<xsl:value-of select="@name"/>];<br/>
	</xsl:template>
	<xsl:template match="index">
		case INDEX_<xsl:value-of select="@name"/>:<br/>
		<xsl:apply-templates select="tableref" />
		return n;<br/>
	</xsl:template>
	<xsl:template match="tableref">
		n += rows[TABLE_<xsl:value-of select="."/>];<br/>
	</xsl:template>
-->
</xsl:stylesheet>
