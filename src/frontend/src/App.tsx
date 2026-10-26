import { SessionHydrator } from "./app/SessionHydrator";
import { AppRoutes } from "./app/AppRoutes";
import { ThemeSync } from "./app/ThemeSync";
import "./App.css";

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
