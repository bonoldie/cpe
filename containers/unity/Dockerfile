FROM ubuntu:22.04
SHELL ["/bin/bash", "-c"]
ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update 

# Workspace setup
RUN mkdir /workspace
COPY ./install.sh /workspace/install.sh

WORKDIR /workspace
RUN chmod u+x ./install.sh

# Run installer
RUN ./install.sh

# Libpython fix
RUN ln -s /usr/lib/x86_64-linux-gnu/libpython3.8.so.1.0 /usr/lib/x86_64-linux-gnu/libpython3.6m.so.1.0

# Setup entrypoint
ADD entrypoint.sh /entrypoint.sh
RUN chmod u+x /entrypoint.sh
ENTRYPOINT [ "/entrypoint.sh" ]