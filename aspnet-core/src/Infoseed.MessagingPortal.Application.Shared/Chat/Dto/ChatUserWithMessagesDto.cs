using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Chat.Dto
{
    public class ChatUserWithMessagesDto : ChatUserDto
    {
        public List<ChatMessageDto> Messages { get; set; }
    }
}