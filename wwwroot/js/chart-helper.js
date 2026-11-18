// Lightweight, offline-ready canvas chart helper matching NevBank UI
window.NevCharts = {
    drawDonut: function(canvasId, data, centerText) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        const width = canvas.width = canvas.parentElement.clientWidth || 240;
        const height = canvas.height = 240;
        
        const centerX = width / 2;
        const centerY = height / 2;
        const radius = Math.min(centerX, centerY) - 20;
        const innerRadius = radius * 0.68;
        
        const total = Object.values(data).reduce((a, b) => a + b, 0);
        if (total === 0) {
            ctx.fillStyle = "#94a3b8";
            ctx.font = "14px sans-serif";
            ctx.textAlign = "center";
            ctx.fillText("Belum ada data", centerX, centerY);
            return;
        }

        const colors = {
            "A": "#005743", // Deep emerald
            "B": "#10b981", // Mint emerald
            "C": "#f59e0b", // Amber
            "D": "#ef4444"  // Red
        };

        let startAngle = -Math.PI / 2;

        for (const [key, value] of Object.entries(data)) {
            if (value <= 0) continue;
            const sliceAngle = (value / total) * 2 * Math.PI;
            ctx.beginPath();
            ctx.arc(centerX, centerY, radius, startAngle, startAngle + sliceAngle);
            ctx.arc(centerX, centerY, innerRadius, startAngle + sliceAngle, startAngle, true);
            ctx.closePath();
            ctx.fillStyle = colors[key] || "#cbd5e1";
            ctx.fill();
            startAngle += sliceAngle;
        }

        // Center cutout
        ctx.beginPath();
        ctx.arc(centerX, centerY, innerRadius - 2, 0, 2 * Math.PI);
        ctx.fillStyle = "#ffffff";
        ctx.fill();

        // Center text
        ctx.fillStyle = "#1e293b";
        ctx.font = "bold 22px sans-serif";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillText(centerText || total.toString(), centerX, centerY - 6);

        ctx.fillStyle = "#64748b";
        ctx.font = "11px sans-serif";
        ctx.fillText("TOTAL DATA", centerX, centerY + 14);
    },

    drawBar: function(canvasId, labels, values) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        const containerWidth = canvas.parentElement.clientWidth || 600;
        const width = canvas.width = containerWidth;
        const height = canvas.height = 320;

        const padding = { top: 30, right: 30, bottom: 65, left: 50 };
        const chartWidth = width - padding.left - padding.right;
        const chartHeight = height - padding.top - padding.bottom;

        ctx.clearRect(0, 0, width, height);

        // Max value scale (0 to 100)
        const maxVal = 100;

        // Draw horizontal grid lines
        ctx.strokeStyle = "#e2e8f0";
        ctx.lineWidth = 1;
        ctx.fillStyle = "#94a3b8";
        ctx.font = "11px sans-serif";
        ctx.textAlign = "right";

        for (let i = 0; i <= 5; i++) {
            const yVal = i * 20;
            const y = padding.top + chartHeight - (yVal / maxVal) * chartHeight;
            
            ctx.beginPath();
            ctx.moveTo(padding.left, y);
            ctx.lineTo(width - padding.right, y);
            ctx.stroke();

            ctx.fillText(yVal.toString(), padding.left - 10, y + 4);
        }

        if (!values || values.length === 0) {
            ctx.fillStyle = "#94a3b8";
            ctx.font = "14px sans-serif";
            ctx.textAlign = "center";
            ctx.fillText("Tidak ada data untuk ditampilkan", width / 2, height / 2);
            return;
        }

        const barWidth = Math.min(48, Math.max(16, (chartWidth / values.length) * 0.55));
        const gap = chartWidth / values.length;

        values.forEach((val, idx) => {
            const x = padding.left + (idx * gap) + (gap - barWidth) / 2;
            const barHeight = (val / maxVal) * chartHeight;
            const y = padding.top + chartHeight - barHeight;

            // Gradient bar
            const grad = ctx.createLinearGradient(0, y, 0, y + barHeight);
            grad.addColorStop(0, '#005743');
            grad.addColorStop(1, '#10b981');

            ctx.fillStyle = grad;
            // Rounded top bar
            const radius = Math.min(6, barWidth / 2);
            ctx.beginPath();
            ctx.moveTo(x, y + barHeight);
            ctx.lineTo(x, y + radius);
            ctx.quadraticCurveTo(x, y, x + radius, y);
            ctx.lineTo(x + barWidth - radius, y);
            ctx.quadraticCurveTo(x + barWidth, y, x + barWidth, y + radius);
            ctx.lineTo(x + barWidth, y + barHeight);
            ctx.closePath();
            ctx.fill();

            // Value text on top of bar
            ctx.fillStyle = "#0f172a";
            ctx.font = "bold 11px sans-serif";
            ctx.textAlign = "center";
            ctx.fillText(val.toFixed(1), x + barWidth / 2, y - 8);

            // Label text below bar
            ctx.save();
            ctx.translate(x + barWidth / 2, padding.top + chartHeight + 15);
            ctx.rotate(-Math.PI / 6);
            ctx.fillStyle = "#475569";
            ctx.font = "10px sans-serif";
            ctx.textAlign = "right";
            const lbl = labels[idx] || "";
            ctx.fillText(lbl.length > 15 ? lbl.substring(0, 14) + "…" : lbl, 0, 0);
            ctx.restore();
        });
    }
};
