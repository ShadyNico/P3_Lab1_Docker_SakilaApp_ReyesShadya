# Laboratorio Docker Swarm de SakilaApp

## Requisitos externos

- Un manager y al menos un worker, en máquinas o VM diferentes.
- Docker Engine en todos los nodos.
- Comunicación TCP 2377, TCP/UDP 7946 y UDP 4789.
- La imagen `sakilaapp:compose` cargada manualmente en cada nodo.
- El nodo persistente debe conservar la etiqueta `sakila.postgres-data=true`.

No se publica la imagen en registros externos. `scripts/03-Export-SakilaImage.ps1`
permite exportarla; en cada worker debe cargarse con `docker image load`.

## Orden de ejecución

1. En el manager: `./scripts/01-Initialize-SwarmManager.ps1 -AdvertiseAddress IP_MANAGER`.
2. Mostrar el comando temporal: `./scripts/02-Show-WorkerJoinCommand.ps1`.
3. Ejecutar ese comando directamente en cada worker.
4. Exportar y cargar `sakilaapp:compose` en todos los workers.
5. Respaldar: `./scripts/11-Backup-HostPostgres.ps1`.
6. Desplegar: `./scripts/04-Deploy-SwarmStack.ps1 -PostgresNode NODO_PERSISTENTE`.
7. En el nodo PostgreSQL, restaurar: `./scripts/12-Restore-StackPostgres.ps1`.
8. Verificar: `./scripts/05-Verify-Swarm.ps1`.
9. Escalar a cuatro: `./scripts/06-Scale-SakilaToFour.ps1`.
10. Reducir a tres: `./scripts/07-Scale-SakilaToThree.ps1`.
11. Drenar un worker de aplicación y verificar recuperación.
12. Reactivarlo con `./scripts/09-Activate-Worker.ps1`.

## Persistencia y autenticación

PostgreSQL tiene exactamente una réplica y un volumen local fijado al nodo etiquetado.
Un volumen local no migra si ese nodo se pierde.

`sakila_keys` permite compartir las claves entre réplicas que coinciden en el mismo nodo,
pero Docker no comparte volúmenes locales entre máquinas. Para cookies consistentes en un
clúster multinodo real se necesita un volumen compartido compatible con todos los nodos
(por ejemplo NFS) o un proveedor externo de Data Protection. No se configuró una dirección
NFS ficticia.

La base PostgreSQL creada por la imagen oficial empieza vacía. El catálogo Sakila existente
debe respaldarse y restaurarse en el volumen del stack. No se considera válida la prueba del
catálogo hasta completar esa migración.

## Evidencias requeridas

Capture: `docker node ls`; `docker-stack.yml`; despliegue inicial; servicios 2/2 y 1/1;
distribución por nodo; navegador; escalamiento 4/4; PostgreSQL 1/1; reducción 3/3;
worker antes/durante drain; tarea reemplazada; estado final 3/3 y 1/1; acceso final web.
