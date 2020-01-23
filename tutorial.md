Helm Charts https://www.youtube.com/watch?v=9cwjtN3gkD4

helm create <helm-name>

helm dependency update .


Rancher Schedule Pod https://rancher.com/docs/rancher/v2.x/en/v1.6-migration/schedule-workloads/

Local Registry:
 docker run -d -p 5000:5000 --restart always --name registry registry:2

Share Local Registry with Minikube https://blog.hasura.io/sharing-a-local-registry-for-minikube-37c7240d0615/