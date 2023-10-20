class SnowflakeIdGenerator {
    constructor(epoch) {
        this.epoch = BigInt(epoch);
        this.sequence = 0n;
        this.lastTimestamp = 0n;
    }

    generateId() {
        let timestamp = BigInt(Date.now()) - this.epoch;

        if (timestamp < this.lastTimestamp) throw new Error('Clock moved backwards.');

        if (timestamp === this.lastTimestamp) {
            this.sequence = (this.sequence + 1n) & 4095n;
            if (this.sequence === 0n) timestamp = this.waitNextMillis();
        } else {
            this.sequence = 0n;
        }

        this.lastTimestamp = timestamp;

        return ((timestamp << 12n) + this.sequence).toString();
    }

    waitNextMillis() {
        let timestamp = BigInt(Date.now()) - this.epoch;
        while (timestamp <= this.lastTimestamp) timestamp = BigInt(Date.now()) - this.epoch;
        return timestamp;
    }
}

let nrcSnowflake = {
    IdGeneratorInstance: new SnowflakeIdGenerator(Date.parse('2022-06-06T00:00:00Z')),
    id: () => nrcSnowflake.IdGeneratorInstance.generateId() * 1
}

Object.assign(window, { nrcSnowflake });
export { nrcSnowflake };