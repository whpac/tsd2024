class RandomList<T> {
    private list: T[] = [];

    add(item: T): void {
        let r = Math.random();
        if(r < 0.5) {
            this.list.unshift(item);
        } else {
            this.list.push(item);
        }
    }

    get(index: number): T {
        let r = Math.random() * (index + 1);
        r = Math.floor(r);
        if(r > index) r = index;
        return this.list[r];
    }

    get length(): number {
        return this.list.length;
    }
}