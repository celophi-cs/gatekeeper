import mongoose, { Schema } from 'mongoose';

const ClientSchema = new Schema({
  clientId: { type: String, required: true, unique: true },
  clientSecret: { type: String, required: true },
  redirectUris: { type: [String], required: true },
  name: { type: String },
}, { timestamps: true });

export default mongoose.model('Client', ClientSchema);
