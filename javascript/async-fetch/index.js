const https = require("https");

function get(url) {
  return new Promise((resolve, reject) => {
    https.get(url, (res) => {
      let data = "";
      res.on("data", (chunk) => (data += chunk));
      res.on("end", () => {
        try {
          resolve(JSON.parse(data));
        } catch {
          resolve(data);
        }
      });
    }).on("error", reject);
  });
}

async function main() {
  const users = await get("https://jsonplaceholder.typicode.com/users");
  
  for (const user of users.slice(0, 3)) {
    const posts = await get(
      `https://jsonplaceholder.typicode.com/posts?userId=${user.id}`
    );
    console.log(`${user.name} — ${posts.length} posts`);
  }
}

main().catch(console.error);
