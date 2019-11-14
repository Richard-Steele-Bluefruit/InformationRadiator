<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output method="text" encoding="utf-8"/>

  <xsl:variable name='nl'>
    <xsl:text>&#xa;</xsl:text>
  </xsl:variable>

  <xsl:template match="/">

    <!-- Line coverage based on Sequence Points -->
    <xsl:text>##teamcity[buildStatisticValue key='CodeCoverageAbsLTotal' value='</xsl:text>
    <xsl:value-of select="/CoverageSession/Summary/@numSequencePoints" />
    <xsl:text>']</xsl:text>
    <xsl:value-of select="$nl"/>

    <xsl:text>##teamcity[buildStatisticValue key='CodeCoverageAbsLCovered' value='</xsl:text>
    <xsl:value-of select="/CoverageSession/Summary/@visitedSequencePoints" />
    <xsl:text>']</xsl:text>
    <xsl:value-of select="$nl"/>

    <!-- Class coverage -->
    <xsl:text>##teamcity[buildStatisticValue key='CodeCoverageAbsCTotal' value='</xsl:text>
    <xsl:value-of select="/CoverageSession/Summary/@numClasses" />
    <xsl:text>']</xsl:text>
    <xsl:value-of select="$nl"/>

    <xsl:text>##teamcity[buildStatisticValue key='CodeCoverageAbsCCovered' value='</xsl:text>
    <xsl:value-of select="/CoverageSession/Summary/@visitedClasses" />
    <xsl:text>']</xsl:text>
    <xsl:value-of select="$nl"/>

    <!-- Method coverage -->
    <xsl:text>##teamcity[buildStatisticValue key='CodeCoverageAbsMTotal' value='</xsl:text>
    <xsl:value-of select="/CoverageSession/Summary/@numMethods" />
    <xsl:text>']</xsl:text>
    <xsl:value-of select="$nl"/>

    <xsl:text>##teamcity[buildStatisticValue key='CodeCoverageAbsMCovered' value='</xsl:text>
    <xsl:value-of select="/CoverageSession/Summary/@visitedMethods" />
    <xsl:text>']</xsl:text>
    <xsl:value-of select="$nl"/>

  </xsl:template>
</xsl:stylesheet>
