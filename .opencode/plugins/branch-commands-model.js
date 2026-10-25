// A command's static `model` field cannot fall back to the current session model.
export const BranchCommandsModel = async ({ client }) => {
  const pending = new Set()

  return {
    "command.execute.before": async ({ command, sessionID }) => {
      if (
        command === "new-branch" ||
        command === "suggest-branch-names" ||
        command === "commit-message-staged"
      ) pending.add(sessionID)
    },
    "chat.message": async ({ sessionID }, { message }) => {
      if (!pending.delete(sessionID)) return

      try {
        const result = await client.config.providers()
        const providers = result.data?.providers ?? []
        const models = providers.flatMap((provider) =>
          Object.keys(provider.models ?? {}).map((modelID) => ({ providerID: provider.id, modelID })),
        )
        const deepseek = models.find(({ providerID, modelID }) =>
          providerID === "opencode" && /deepseek/i.test(modelID),
        ) ?? models.find(({ providerID, modelID }) => /deepseek/i.test(`${providerID}/${modelID}`))
        if (deepseek) message.model = deepseek
      } catch {
        // Leave the command on the model already selected in the session.
      }
    },
  }
}
