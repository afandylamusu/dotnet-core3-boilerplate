{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "mdm-app-chart.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "mdm-app-chart.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "mdm-app-chart.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "mdm-app-chart.labels" -}}
helm.sh/chart: {{ include "mdm-app-chart.chart" . }}
{{ include "mdm-app-chart.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "mdm-app-chart.selectorLabels" -}}
app.kubernetes.io/name: {{ include "mdm-app-chart.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "mdm-app-chart.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
    {{ default (include "mdm-app-chart.fullname" .) .Values.serviceAccount.name }}
{{- else -}}
    {{ default "default" .Values.serviceAccount.name }}
{{- end -}}
{{- end -}}


{{- define "cp-kafka.fullname" -}}
{{- printf "%s-cp-kafka" .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "cp-schema-registry.fullname" -}}
{{- printf "%s-cp-schema-registry" .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "mdm-grpc-chart.fullname" -}}
{{- printf "%s-mdm-grpc-chart" .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- end -}}