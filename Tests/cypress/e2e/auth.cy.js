describe("API Auth", () => {
  const baseUrl = "http://localhost:5000";
  const user = {
    name: "Teste Cypress",
    email: `cypress_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  let token;

  it("POST /api/auth/register - Deve registrar um novo usuário", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, user).then(
      (response) => {
        expect(response.status).to.equal(201);
        expect(response.body.token).to.exist;
        expect(response.body.user.email).to.equal(user.email);
        token = response.body.token;
      }
    );
  });

  it("POST /api/auth/login - Deve autenticar usuário existente", () => {
    cy.request("POST", `${baseUrl}/api/auth/login`, {
      email: user.email,
      password: user.password,
    }).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body.token).to.exist;
      token = response.body.token;
    });
  });

  it("GET /api/auth/me - Deve negar acesso sem token", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/auth/me`,
      failOnStatusCode: false,
    }).then((response) => {
      expect(response.status).to.be.oneOf([401, 403]);
    });
  });

  it("GET /api/auth/me - Deve retornar dados do usuário com token válido", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/auth/me`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body.email).to.equal(user.email);
    });
  });
});
