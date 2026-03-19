import OpenAI from 'openai';
import 'dotenv/config';
import { pool } from './db.js';

const openai = new OpenAI({ apiKey: process.env.OPENAI_API_KEY });

export async function getDailyPrompts() {
    try {
        const completion = await openai.chat.completions.create({
            model: "gpt-4o",
            messages: [{ role: "user", content: `You are a journal prompt generator.
                        Your job is to produce thoughtful, reflective journal entry prompts that help the user reflect on multiple areas of life.
                        Each response should include 2-3 journal prompts grouped across these categories:
                        - Spiritual (or values/purpose if the user is non-religious)
                        - Mental (thought patterns, mindset, learning, clarity)
                        - Emotional (feelings, emotional processing, relationships with emotions)
                        - School/Work (productivity, goals, performance, growth)
                        - Physical (health, fitness, sleep, nutrition, energy)
                        - Relationships (friends, family, social life, communication)
                        - Personal Growth (habits, discipline, identity, long-term direction)
                        Rules:
                        - Each prompt should be open-ended and reflective (not yes/no questions).
                        - Avoid repetition across categories.
                        - Keep tone supportive, calm, and introspective.
                        - Do not give advice—only questions.
                        - Vary prompts daily so they do not feel repetitive.
                        - You may include 2 optional “wildcard” prompt that does not fit any category but encourages deep reflection.
                        Output format:
                        - Return ONLY valid JSON:
                            {
                            "prompts": [
                                "prompt 1",
                                "prompt 2",
                                "prompt 3"
                            ]}` }]
        });
    
    const raw = completion.choices[0].message.content;
    const cleaned = raw.replace(/```json\n?|```/g, '').trim();
    const data = JSON.parse(cleaned)
    const prompts = data.prompts;

    for (const prompt of prompts){
        await pool.query('INSERT INTO prompts (prompt, date) VALUES (?, NOW())',
            [prompt]
        )
    }
    console.log("Prompts inserted:", prompts);

    } catch(error){
        console.error("Error in AI task:", error);
        throw error;
    }
}