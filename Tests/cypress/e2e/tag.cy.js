describe("API Tag", () => {
  const baseUrl = "http://localhost:5000";

  const adminUser = {
    name: "Admin Tags Cypress",
    email: `admin_tags_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  let adminToken;
  let tagId;
  const tagName = `Tag Cypress ${Date.now()}`;

  it("GET /api/tag - Deve listar tags (público)", () => {
    cy.request("GET", `${baseUrl}/api/tag`).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body).to.be.an("array");
    });
  });

  it("GET /api/tag/{id} - Deve permitir buscar tag pública", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/tag/1`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("GET /api/tag/name/{name} - Deve permitir buscar tag por nome (público)", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/tag/name/pizza`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("POST /api/auth/register - Deve registrar ADMIN para gerenciar tags", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, adminUser).then(
      (response) => {
        expect(response.status).to.equal(201);
        adminToken = response.body.token;
      }
    );
  });

  it("POST /api/tag - Deve criar nova tag (rota pública)", () => {
    const body = {
      name: tagName,
      description: "Tag criada via Cypress",
    };

    cy.request("POST", `${baseUrl}/api/tag`, body).then((response) => {
      expect(response.status).to.equal(201);
      expect(response.body.id).to.exist;
      expect(response.body.name).to.equal(tagName);
      tagId = response.body.id;
    });
  });

  it("GET /api/tag/{id} - Deve retornar tag recém-criada (público)", function() {
    cy.request("GET", `${baseUrl}/api/tag/${tagId}`).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body.id).to.equal(tagId);
      expect(response.body.name).to.equal(tagName);
    });
  });

  it("PUT /api/tag/{id} - Deve negar atualização sem token (somente ADMIN)", function() {
    const body = {
      name: tagName + " Editado",
      description: "Editado sem token",
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/tag/${tagId}`,
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("PUT /api/tag/{id} - Deve atualizar tag com ADMIN autenticado (ou falhar se role não for ADMIN)", function() {
    const body = {
      name: tagName + " Editado",
      description: "Tag editada via Cypress",
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/tag/${tagId}`,
      headers: {
        Authorization: `Bearer ${adminToken}`,
      },
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 403]).to.include(response.status);
    });
  });

  it("DELETE /api/tag/{id} - Deve negar exclusão sem token (somente ADMIN)", function() {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/tag/${tagId}`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("DELETE /api/tag/{id} - Deve excluir tag com ADMIN autenticado (ou falhar se role não for ADMIN)", function() {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/tag/${tagId}`,
      headers: {
        Authorization: `Bearer ${adminToken}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([204, 403]).to.include(response.status);
    });
  });
});
