import { create } from "domain";
import mongoose from "mongoose";

const collectionSchema = new mongoose.Schema({
    title:{
        type: String,
        require: true,
        unique:true,
    },
    description:String,
    image:{
        type: String,
        require: true,
    },
    products:[
        {
            type: mongoose.Schema.Types.ObjectId,
            ref: "Product",
        }
    ],
    createdAt: {
        type: Date,
        default: Date.now,
    },
    UpdatedAt: {
        type: Date,
        default: Date.now,
    },
})

const Collection = mongoose.models.Collection || mongoose.model("Collection", collectionSchema);

export default Collection;