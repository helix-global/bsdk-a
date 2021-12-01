<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="member">
    <xsl:element name="member">
      <xsl:apply-templates select="@*"/>
      <xsl:choose>
        <xsl:when test="summary[@lang='en']">
          <xsl:apply-templates select="summary[@lang='en']"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="summary[not(@lang)]"/>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:apply-templates select="param"/>
      <xsl:apply-templates select="returns"/>
      <xsl:apply-templates select="exception"/>
      <xsl:apply-templates select="filterpriority"/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
