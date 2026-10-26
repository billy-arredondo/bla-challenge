import { SessionHydrator } from "./app/SessionHydrator";
import { AppRoutes } from "./app/AppRoutes";
import { ThemeSync } from "./app/ThemeSync";
import "./styles/index.css";

function App() {
  return (
    <>
      <SessionHydrator />
      <ThemeSync />
      <AppRoutes />
    </>
  );
}

export default App;
