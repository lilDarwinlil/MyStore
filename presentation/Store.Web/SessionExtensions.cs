﻿using Microsoft.AspNetCore.Http;
using Store.Web.Models;
using System.IO;
using System.Text;

namespace Store.Web
{
    public static class SessionExtensions
    {
        const string key = "Cart";

        public static void Set(this ISession session, Cart value)
        {
            if (value == null)
                return;

            using(var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(value.OrderId);
                writer.Write(value.TotalCount);
                writer.Write(value.TotalPrice);

                session.Set(key, stream.ToArray());
            }
        }

        public static bool TryGetCart(this ISession session, out Cart value)
        {
            if(session.TryGetValue(key, out byte[] buf))
            {
                using (var stream = new MemoryStream(buf))
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var orderId = reader.ReadInt32();
                    var totalCount = reader.ReadInt32();
                    var totalPrice = reader.ReadDecimal();

                    value = new Cart(orderId)
                    {
                        TotalCount = totalCount,
                        TotalPrice = totalPrice,
                    };

                    return true;
                }
            }
            value = null;
            return false;
        }
    }
}
