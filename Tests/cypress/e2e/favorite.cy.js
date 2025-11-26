describe("API Favorite", () => {
  const baseUrl = "http://localhost:5000";

  const user = {
    name: "Teste Favorite",
    email: `cypress_fav_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  let token;
  let favoriteId;
  const restaurantId = 1;

  it("POST /api/auth/register - Deve registrar usuário para usar favoritos", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, user).then(
      (response) => {
        expect(response.status).to.equal(201);
        token = response.body.token;
      }
    );
  });

  it("GET /api/favorite - Deve listar todos os favoritos (público)", () => {
    cy.request("GET", `${baseUrl}/api/favorite`).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body).to.be.an("array");
    });
  });

  it("GET /api/favorite/status/{restaurantId} - Deve negar acesso sem token", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/favorite/status/${restaurantId}`,
      failOnStatusCode: false,
    }).then((response) => {
      expect(response.status).to.be.oneOf([401, 403]);
    });
  });

  it("POST /api/favorite - Deve adicionar favorito para usuário autenticado", function() {
    const body = { restaurantId };
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/favorite`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body,
    }).then((response) => {
      expect(response.status).to.equal(201);
      expect(response.body.id).to.exist;
      expect(response.body.restaurantId).to.equal(restaurantId);
      favoriteId = response.body.id;
    });
  });

  it("GET /api/favorite/my-favorites - Deve listar favoritos do usuário autenticado", function() {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/favorite/my-favorites`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body).to.be.an("array");
    });
  });

  it("GET /api/favorite/status/{restaurantId} - Deve retornar status de favorito para usuário autenticado", function() {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/favorite/status/${restaurantId}`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }).then((response) => {
      expect(response.status).to.equal(200);
      // exemplo: response.body.isFavorite === true/false
      expect(response.body).to.have.property("isFavorite");
    });
  });

  it("POST /api/favorite/toggle - Deve alternar favorito (add/remove) para usuário autenticado", function() {
    const body = { restaurantId };
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/favorite/toggle`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body,
    }).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body).to.have.property("isFavorite");
      // Após toggle, adicionar de volta para garantir que existe para o próximo teste
      if (!response.body.isFavorite) {
        cy.request({
          method: "POST",
          url: `${baseUrl}/api/favorite`,
          headers: {
            Authorization: `Bearer ${token}`,
          },
          body,
        }).then((addResponse) => {
          favoriteId = addResponse.body.id;
        });
      }
    });
  });

  it("DELETE /api/favorite/{id} - Deve remover favorito do próprio usuário", function() {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/favorite/${favoriteId}`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([204, 404]).to.include(response.status);
    });
  });

  it("DELETE /api/favorite/restaurant/{restaurantId} - Deve remover favorito pelo restaurantId para usuário autenticado", function() {
    // Primeiro adiciona um favorito
    const body = { restaurantId };
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/favorite`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body,
      failOnStatusCode: false,
    }).then(() => {
      cy.request({
        method: "DELETE",
        url: `${baseUrl}/api/favorite/restaurant/${restaurantId}`,
        headers: {
          Authorization: `Bearer ${token}`,
        },
        failOnStatusCode: false,
      }).then((response) => {
        expect([204, 404]).to.include(response.status);
      });
    });
  });
});
