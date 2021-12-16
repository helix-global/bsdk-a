<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="Certificates">
    <html>
      <body>
        <style>
          td
          {
          font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode';
          font-size: x-small;
          }
          table
          {
          padding: 0px;
          margin: 0px;
          border-collapse: collapse;
          border-spacing: 0px;
          }
          .td1
          {
          border-style: solid;
          border-width: 1px 1px 1px 1px;
          border-color: #000000;
          border-collapse: collapse
          }
        </style>
        <table style="width: 100%">
          <tr>
            <td colspan="7"><b>Exceptions:</b></td>
          </tr>
          <xsl:for-each select="Exception">
            <tr>
              <td width="20px"><xsl:value-of select="position()"/></td>
              <td colspan="6"><xsl:call-template name="Exception"/></td>
            </tr>
          </xsl:for-each>
          <tr>
            <td colspan="7"><b>Certificates:</b></td>
          </tr>
          <tr style="background-color:#9999FF">
            <td class="td1" width="20px">#</td>
            <td class="td1">Subject</td>
            <td class="td1">Issuer</td>
            <td class="td1">NotBefore</td>
            <td class="td1">NotAfter</td>
            <td class="td1">SerialNumber</td>
            <td class="td1">Thumbprint</td>
          </tr>
          <xsl:for-each select="Certificate">
            <tr>
              <td class="td1"><xsl:value-of select="position()"/></td>
              <xsl:call-template name="CertificateShort"/>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="CertificateShort">
    <td class="td1"><xsl:value-of select="Certificate.Subject"/></td>
    <td class="td1"><xsl:value-of select="Certificate.Issuer"/></td>
    <td class="td1"><xsl:value-of select="@NotBefore"/></td>
    <td class="td1"><xsl:value-of select="@NotAfter"/></td>
    <td class="td1"><xsl:value-of select="@SerialNumber"/></td>
    <td class="td1"><xsl:value-of select="@Thumbprint"/></td>
  </xsl:template>
  <xsl:template name="Exception">
    <table style="width: 100%">
      <tr>
        <td colspan="2"><xsl:value-of select="Message"/></td>
      </tr>
      <tr>
        <td style="width: 10px">Type:</td>
        <td><xsl:value-of select="@Type"/></td>
      </tr>
      <xsl:if test="Exceptions">
        <tr>
          <td colspan="2"><b>Exceptions (<xsl:value-of select="Exceptions/@Count"/>)</b></td>
        </tr>
        <tr>
          <td colspan="2">
            <table style="width: 100%">
              <xsl:for-each select="Exceptions/Exception">
                <tr>
                  <td class="td1" width="20px"><xsl:value-of select="position()"/></td>
                  <td class="td1"><xsl:call-template name="Exception"/></td>
                </tr>
              </xsl:for-each>
            </table>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="Data">
        <tr>
          <td colspan="2"><b>Data (<xsl:value-of select="Data/@Count"/>)</b></td>
        </tr>
        <tr>
          <td>
            <table style="width: 100%">
              <xsl:for-each select="Data/KeyValuePair">
                <tr>
                  <td class="td1"><xsl:value-of select="@Key"/></td>
                  <td>
                    <xsl:choose>
                      <xsl:when test="Value">
                        <xsl:apply-templates select="*"/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="@Value"/>
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <xsl:template match="Certificate">
    <table width="100%">
      <tr>
        <td class="td1">Version</td>
        <td class="td1"><xsl:value-of select="@Version"/></td>
      </tr>
      <tr>
        <td class="td1">Thumbprint</td>
        <td class="td1"><xsl:value-of select="@Thumbprint"/></td>
      </tr>
      <tr>
        <td class="td1">NotBefore</td>
        <td class="td1"><xsl:value-of select="@NotBefore"/></td>
      </tr>
      <tr>
        <td class="td1">NotAfter</td>
        <td class="td1"><xsl:value-of select="@NotAfter"/></td>
      </tr>
      <tr>
        <td class="td1">SerialNumber</td>
        <td class="td1"><xsl:value-of select="@SerialNumber"/></td>
      </tr>
      <tr>
        <td class="td1">KeySpec</td>
        <td class="td1"><xsl:value-of select="@KeySpec"/></td>
      </tr>
      <tr>
        <td class="td1">Issuer</td>
        <td class="td1">
          <table width="100%">
            <tr>
              <td colspan="2"><xsl:value-of select="Certificate.Issuer"/></td>
            </tr>
              <xsl:for-each select="Certificate.Issuer/TypeValuePair">
                <tr>
                  <td class="td1"><xsl:value-of select="@Type"/></td>
                  <td class="td1"><xsl:value-of select="@Value"/></td>
                </tr>
              </xsl:for-each>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--<xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>-->
</xsl:stylesheet>
