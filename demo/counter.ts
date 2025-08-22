export function incrementCounter(count: number): number {
    return count + 1;
}

export function showCountMessage(count: number): string {
    alert(`Counter value is: ${count}`);
    return `Message shown for count: ${count}`;
}

interface CounterUtils {
    double: (count: number) => number;
    reset: () => number;
    isEven: (count: number) => boolean;
    format: (count: number) => string;
}

export function getCounterUtils(): CounterUtils {
    return {
        double: (count: number): number => count * 2,
        reset: (): number => 0,
        isEven: (count: number): boolean => count % 2 === 0,
        format: (count: number): string => `Count: ${count}`
    };
}

export function asyncOperation(count: number): Promise<number> {
    return new Promise<number>((resolve) => {
        setTimeout(() => {
            resolve(count * 10);
        }, 1000);
    });
}

// 演示回调函数支持
export function processWithCallback(count: number, callback: (result: number) => void): number {
    const result: number = count + 100;
    callback(result);
    return result;
}

