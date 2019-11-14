<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output method="text" encoding="utf-8"/>

  <xsl:variable name='nl'>
    <xsl:text>&#xa;</xsl:text>
  </xsl:variable>

  <xsl:template match="/">

    <xsl:variable name="totalMetrics" select="count(/sourcemonitor_metrics/project/metric_names/metric_name)" />

    <xsl:for-each select="/sourcemonitor_metrics/project/metric_names/metric_name">
      <xsl:text>##teamcity[buildStatisticValue key='SM_</xsl:text>
      <xsl:value-of select="translate(current(), ' ', '_')"/>
      <xsl:text>' value='</xsl:text>
      <xsl:variable name="id" select="@id"/>

      <xsl:for-each select="/sourcemonitor_metrics/project/checkpoints/checkpoint">
        <xsl:for-each select="metrics/metric">
          <xsl:if test="@id = $id">
            <xsl:value-of select="current()"/>
          </xsl:if>
        </xsl:for-each>
        <xsl:text>']</xsl:text>
        <xsl:value-of select="$nl"/>
      </xsl:for-each>

    </xsl:for-each>

  </xsl:template>
</xsl:stylesheet>
