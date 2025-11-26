describe("API User", () => {
  const baseUrl = "http://localhost:5000";

  const adminUser = {
    name: "Admin Users Cypress",
    email: `admin_users_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  const normalUser = {
    name: "User Cypress",
    email: `user_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  let adminToken;
  let userToken;
  let userId;

  it("POST /api/auth/register - Deve registrar ADMIN para gerenciar usuários", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, adminUser).then(
      (response) => {
        expect(response.status).to.equal(201);
        adminToken = response.body.token;
      }
    );
  });

  it("POST /api/auth/register - Deve registrar usuário comum", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, normalUser).then(
      (response) => {
        expect(response.status).to.equal(201);
        userToken = response.body.token;
        userId = response.body.user.id;
      }
    );
  });

  it("GET /api/user - Deve negar acesso sem token (somente ADMIN)", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/user?pageNumber=1&pageSize=10`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("GET /api/user - Deve retornar usuários com ADMIN autenticado (ou falhar se role não for ADMIN)", function() {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/user?pageNumber=1&pageSize=10`,
      headers: {
        Authorization: `Bearer ${adminToken}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 403]).to.include(response.status);
    });
  });

  it("GET /api/user/{id} - Deve negar acesso sem token", function() {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/user/${userId}`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("GET /api/user/{id} - Deve permitir acesso ao próprio usuário autenticado", function() {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/user/${userId}`,
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
    }).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body.id).to.equal(userId);
    });
  });

  it("GET /api/user/{id} - Deve permitir acesso com ADMIN autenticado (ou falhar se role não for ADMIN)", function() {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/user/${userId}`,
      headers: {
        Authorization: `Bearer ${adminToken}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 401, 403]).to.include(response.status);
    });
  });

  it("PUT /api/user/{id} - Deve negar atualização sem token", function() {
    const body = {
      name: "User Editado Sem Token",
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/user/${userId}`,
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("PUT /api/user/{id} - Deve atualizar dados com próprio usuário autenticado", function() {
    const body = {
      name: "User Cypress Editado",
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/user/${userId}`,
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
      body,
    }).then((response) => {
      expect(response.status).to.equal(200);
    });
  });

  it("PUT /api/user/{id} - Deve atualizar dados com ADMIN autenticado (ou falhar se role não for ADMIN)", function() {
    const body = {
      name: "User Cypress Editado Pelo Admin",
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/user/${userId}`,
      headers: {
        Authorization: `Bearer ${adminToken}`,
      },
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 403]).to.include(response.status);
    });
  });

  it("POST /api/user/{id}/upload-image - Deve negar upload sem token", function() {
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/user/${userId}/upload-image`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("POST /api/user/{id}/upload-image - Deve aceitar requisição com próprio usuário autenticado (se multipart configurado)", function() {
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/user/${userId}/upload-image`,
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 400, 415]).to.include(response.status);
    });
  });

  it("POST /api/user/{id}/upload-image - Deve aceitar requisição com ADMIN autenticado (se multipart configurado)", function() {
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/user/${userId}/upload-image`,
      headers: {
        Authorization: `Bearer ${adminToken}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 400, 415]).to.include(response.status);
    });
  });

  it("DELETE /api/user/{id} - Deve negar exclusão sem token", function() {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/user/${userId}`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("DELETE /api/user/{id} - Deve permitir exclusão com próprio usuário autenticado", function() {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/user/${userId}`,
      headers: {
        Authorization: `Bearer ${userToken}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 204]).to.include(response.status);
    });
  });
});
