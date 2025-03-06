# 🚀 **Roadmap: Upcoming Features & Enhancements**  

## **🛠 Planned Features & Improvements**  

### ✅ **[v1.5.0] - Full Support for Gemma & Qwen2.5**  
📌 **Objective:** Ensure both models execute tools correctly without unnecessary responses.  
🔹 Fix `qwen2.5` execution issues:  
  - Prevent unnecessary **re-prompts** when a tool should be used.  
  - Improve **parameter handling** to ensure the model properly extracts tool inputs.  
🔹 Validate `gemma` functionality:  
  - Ensure it **detects and executes available tools**.  
  - Improve **response validation** to prevent incorrect outputs.  

---

### 📦 **[v1.6.0] - Create a NuGet Package for Reusability**  
📌 **Objective:** Make the tool execution framework reusable across multiple projects.  
🔹 Extract core functionality into a **NuGet package**.  
🔹 Provide an **easy-to-use API** for model selection and tool execution.  
🔹 Ensure **proper documentation & setup instructions** for external integration.  

---

### 🌐 **[v1.7.0] - Implement a REST API Server**  
📌 **Objective:** Create an API that allows users to send queries and get structured responses automatically.  
🔹 Develop a **RESTful API** that:  
  - Accepts user queries via **HTTP requests**.  
  - Automatically selects the **best model and tool** for the query.  
  - Returns **JSON responses** with structured results.  
🔹 Implement **secure access** and optimize **request handling**.  

---

## **📅 Roadmap Timeline**  
| Version | Feature | Status |  
|---------|---------|--------|  
| v1.5.0  | Full support for Gemma & Qwen2.5 | 🔜 Planned |  
| v1.6.0  | Create a reusable NuGet package | 🔜 Planned |  
| v1.7.0  | Implement a REST API server | 🔜 Planned |  

---

## **📌 Next Steps**  
✔ **Refine Mistral & Llama3.2 responses** *(Done in v1.4.0)*  
✔ **Fix execution flow for Qwen2.5 & Gemma** *(Next priority - v1.5.0)*  
✔ **Design the NuGet package structure** *(Parallel development - v1.6.0)*  
✔ **Define API endpoints for the REST server** *(Will start after NuGet release - v1.7.0)*  

