type PageErrorProps = {
  message: string;
};

type EmptyProps = {
  title: string;
  text: string;
};

export function PageLoading() {
  return (
    <div className="center-state">
      <span className="spinner" />
      Loading workspace…
    </div>
  );
}

export function PageError({ message }: PageErrorProps) {
  return (
    <div className="center-state">
      <div className="alert error">{message}</div>
    </div>
  );
}

export function Empty({ title, text }: EmptyProps) {
  return (
    <div className="empty">
      <span>◌</span>
      <strong>{title}</strong>
      <p>{text}</p>
    </div>
  );
}
