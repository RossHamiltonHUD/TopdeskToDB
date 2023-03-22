using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TopdeskDataCache
{
    internal class SqlConnector
    {
        //string connectionString;
        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataReader reader;

        public void OpenConnection(string connectionString = "Persist Security Info=False;" +
                                    "Integrated Security=true;Initial Catalog=TopdeskToDb;server=(local)")
        {
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand(connectionString, conn);
            
            conn.Open();
        }

        public void CloseConnection()
        {
            reader.Close();
            cmd.Dispose();
            conn.Close();
        }

        public int GetExistingTicketCount(string datecode)
        {
            OpenConnection();
            int count = 0;
            cmd = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE Number LIKE '" + datecode + "%';", conn);
            cmd.CommandTimeout = 10;
            reader = cmd.ExecuteReader();
            reader.Read();
            count = (int)reader[0];

            CloseConnection();
            return count;
        }

        public void InsertTickets(List<Ticket> ticketInput)
        {
            OpenConnection();

            foreach (Ticket ticket in ticketInput)
            {
                Console.Write("\rWriting " + ticket.Number + " to database...                   ");

                cmd = new SqlCommand("INSERT INTO Tickets VALUES (@Number,@CallerBranch,@CallerLocation,@Category,@Subcategory,@CallType,@EntryType,@Impact,@Urgency," +
                    "@Priority,@Duration,@ActualDuration,@TargetDate,@FeedbackRating,@Operator,@OperatorGroup,@Supplier,@ProcessingStatus,@Responded,@ResponseDate," +
                    "@Completed,@CompletedDate,@Closed,@ClosedDate,@ClosureCode,@CallDate,@Creator,@CreationDate,@Modifier,@ModificationDate,@MajorCall,@Escalation)",conn);

                cmd.Parameters.AddWithValue("@Number", PrepVar(ticket.Number));
                cmd.Parameters.AddWithValue("@CallerBranch", PrepVar(ticket.CallerBranch));
                cmd.Parameters.AddWithValue("@CallerLocation", PrepVar(ticket.CallerLocation));
                cmd.Parameters.AddWithValue("@Category", PrepVar(ticket.Category));
                cmd.Parameters.AddWithValue("@Subcategory", PrepVar(ticket.Subcategory));
                cmd.Parameters.AddWithValue("@CallType", PrepVar(ticket.CallType));
                cmd.Parameters.AddWithValue("@EntryType", PrepVar(ticket.EntryType));
                cmd.Parameters.AddWithValue("@Impact", PrepVar(ticket.Impact));
                cmd.Parameters.AddWithValue("@Urgency", PrepVar(ticket.Urgency));
                cmd.Parameters.AddWithValue("@Priority", PrepVar(ticket.Priority));
                cmd.Parameters.AddWithValue("@Duration", PrepVar(ticket.Duration));
                cmd.Parameters.AddWithValue("@ActualDuration", PrepVar(ticket.ActualDuration));
                cmd.Parameters.AddWithValue("@TargetDate", PrepVar(ticket.TargetDate));
                cmd.Parameters.AddWithValue("@FeedbackRating", PrepVar(ticket.FeedbackRating));
                cmd.Parameters.AddWithValue("@Operator", PrepVar(ticket.Operator));
                cmd.Parameters.AddWithValue("@OperatorGroup", PrepVar(ticket.OperatorGroup));
                cmd.Parameters.AddWithValue("@Supplier", PrepVar(ticket.Supplier));
                cmd.Parameters.AddWithValue("@ProcessingStatus", PrepVar(ticket.ProcessingStatus));
                cmd.Parameters.AddWithValue("@Responded", PrepVar(ticket.Responded));
                cmd.Parameters.AddWithValue("@ResponseDate", PrepVar(ticket.ResponseDate));
                cmd.Parameters.AddWithValue("@Completed", PrepVar(ticket.Completed));
                cmd.Parameters.AddWithValue("@CompletedDate", PrepVar(ticket.CompletedDate));
                cmd.Parameters.AddWithValue("@Closed", PrepVar(ticket.Closed));
                cmd.Parameters.AddWithValue("@ClosedDate", PrepVar(ticket.ClosedDate)); ;
                cmd.Parameters.AddWithValue("@ClosureCode", PrepVar(ticket.ClosureCode));
                cmd.Parameters.AddWithValue("@CallDate", PrepVar(ticket.CallDate));
                cmd.Parameters.AddWithValue("@Creator", PrepVar(ticket.Creator));
                cmd.Parameters.AddWithValue("@CreationDate", PrepVar(ticket.CreationDate));
                cmd.Parameters.AddWithValue("@Modifier", PrepVar(ticket.Modifier));
                cmd.Parameters.AddWithValue("@ModificationDate", PrepVar(ticket.ModificationDate));
                cmd.Parameters.AddWithValue("@MajorCall", PrepVar(ticket.MajorCall));
                cmd.Parameters.AddWithValue("@Escalation", PrepVar(ticket.Escalation));

                cmd.ExecuteNonQuery();
            }

            CloseConnection();
        }

        public string PrepVar(dynamic varInput, int maxChars = 36)
        {
            if (varInput is null)
            {
                return "";
            }

            else if (varInput.GetType() == typeof(NestedInfo))
            {
                return varInput.Name.Length <= maxChars ? varInput.Name : varInput.Name.Substring(0, maxChars);
            }

            else if (varInput.GetType() == typeof(Caller))
            {
                return varInput.Email.Length <= maxChars ? varInput.Email : varInput.Email.Substring(0, maxChars);
            }

            else if (varInput.GetType() == typeof(OptionalFields))
            {
                return varInput.EscType.Name.Length <= maxChars ? varInput.EscType.Name : varInput.EscType.Name.Substring(0, maxChars);
            }

            else
            {
                return varInput.Length <= maxChars ? varInput : varInput.Substring(0, maxChars);
            }
        }
    }
}
