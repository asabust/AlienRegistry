using System;
using System.Data;
using Game.Runtime.Core.Attributes;
using Game.Runtime.Data;
using System.Collections.Generic;

namespace Game.Runtime.Core.ExcelTableReader
{
    [ExcelSheet("Dialogue")]
    public class DialogueTableReader : IExcelTableReader
    {
        private const int DataStartRow = 3;

        public void Read(DataTable table, ExcelTableContext context)
        {
            ColumnSchema schema = ColumnSchema.Build(table);

            // 用来追踪当前正在构建的 dialogue
            int currentDialogueId = -1;
            bool isTruncated = false; // 当前 dialogue 是否已经被 NextDialogue 截断
            DialogueData currentDialogue = null;

            for (int i = DataStartRow; i < table.Rows.Count; i++)
            {
                var row = new SmartRow(table.Rows[i], schema);

                if (row.IsEmpty("DialogueId")) continue;

                int dialogueId = row.GetInt("DialogueId");

                // 切换到新的 dialogue
                if (dialogueId != currentDialogueId)
                {
                    currentDialogueId = dialogueId;
                    isTruncated = false;

                    if (context.dialogues.ContainsKey(dialogueId))
                        throw new Exception($"对话数据重复 dialogue_id={dialogueId}");

                    currentDialogue = new DialogueData
                    {
                        dialogueId = dialogueId, lines = new List<DialogueLine>(), nextDialogueId = 0
                    };
                    context.dialogues[dialogueId] = currentDialogue;
                }

                // 已被截断，跳过同一 dialogueId 的后续行
                if (isTruncated) continue;

                string speaker = row.GetString("Speaker").Trim();
                string text = row.GetString("Text");
                int nextDialogue = row.GetInt("nextDialogue");

                DialogueType type = speaker.ToLower() switch
                {
                    "choice" => DialogueType.Choice,
                    "black" => DialogueType.Black,
                    "null" => DialogueType.Null,
                    _ => DialogueType.Normal
                };

                if (type == DialogueType.Choice)
                {
                    if (currentDialogue == null || currentDialogue.lines.Count == 0)
                        throw new Exception($"dialogue_id={dialogueId} 的 Choice 前没有任何对话行");

                    // 如果上一行已经是 Choice Line，直接追加
                    DialogueLine lastLine = currentDialogue.lines[^1];

                    if (lastLine?.type == DialogueType.Choice)
                    {
                        lastLine.choices.Add(new Choice { text = text, nextDialogueId = nextDialogue });
                    }
                    else
                    {
                        // 新建一个 Choice Line
                        currentDialogue.lines.Add(new DialogueLine
                        {
                            type = DialogueType.Choice,
                            speakerName = null,
                            text = null,
                            choices = new List<Choice> { new Choice { text = text, nextDialogueId = nextDialogue } }
                        });
                    }
                }
                else
                {
                    currentDialogue.lines.Add(new DialogueLine
                    {
                        type = type,
                        speakerName = type == DialogueType.Normal ? speaker : null,
                        text = text,
                        choices = null
                    });

                    // NextDialogue 有值就截断
                    if (nextDialogue != 0)
                    {
                        currentDialogue.nextDialogueId = nextDialogue;
                        isTruncated = true;
                    }
                }
            }
        }
    }
}