// VPM Listing App
const listingUrl = window.location.href.replace(/\/[^\/]*$/, '/index.json');

// Set URL field value
document.getElementById('vccUrl').value = listingUrl;

// Theme detection
const setTheme = () => {
    const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    document.body.setAttribute('data-theme', isDark ? 'dark' : 'light');
};
setTheme();
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', setTheme);

// Add listing to VCC
document.getElementById('addListingToVCC').addEventListener('click', () => {
    window.location.href = `vcc://vpm/addRepo?url=${encodeURIComponent(listingUrl)}`;
});

// Copy URL
document.getElementById('copyUrl').addEventListener('click', async () => {
    try {
        await navigator.clipboard.writeText(listingUrl);
        const btn = document.getElementById('copyUrl');
        btn.innerHTML = '✓';
        setTimeout(() => {
            btn.innerHTML = `<svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                <path d="M4 4v8h8V4H4zM3 3h10a1 1 0 0 1 1 1v8a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1z"/>
                <path d="M1 1h10v2H3v8H1V1z"/>
            </svg>`;
        }, 2000);
    } catch (e) {
        console.error('Failed to copy:', e);
    }
});

// Help dialog
document.getElementById('helpButton').addEventListener('click', () => {
    document.getElementById('helpDialog').hidden = false;
});

document.getElementById('closeHelp').addEventListener('click', () => {
    document.getElementById('helpDialog').hidden = true;
});

// Load packages from index.json
async function loadPackages() {
    const packageList = document.getElementById('packageList');
    packageList.innerHTML = '<div class="loading">패키지 로딩 중...</div>';

    try {
        const response = await fetch('index.json');
        const data = await response.json();

        packageList.innerHTML = '';

        for (const [packageName, packageData] of Object.entries(data.packages || {})) {
            const versions = packageData.versions || {};
            const versionKeys = Object.keys(versions).sort((a, b) => {
                // Sort versions descending
                return b.localeCompare(a, undefined, { numeric: true });
            });

            if (versionKeys.length === 0) continue;

            const latestVersion = versionKeys[0];
            const pkg = versions[latestVersion];

            const card = document.createElement('div');
            card.className = 'package-card';

            const keywords = pkg.keywords || [];
            const keywordsHtml = keywords.length > 0
                ? `<div class="keywords">${keywords.map(k => `<span class="keyword">${k}</span>`).join('')}</div>`
                : '';

            card.innerHTML = `
                <h3>${pkg.displayName || pkg.name}</h3>
                <div class="package-id">${pkg.name}</div>
                <div class="package-description">${pkg.description || ''}</div>
                <div class="package-version">버전: ${latestVersion}</div>
                <div class="package-actions">
                    <fluent-button appearance="accent" onclick="addPackageToVCC('${pkg.name}')">
                        VCC에 추가
                    </fluent-button>
                    ${pkg.url ? `<fluent-button appearance="outline" onclick="window.open('${pkg.url}', '_blank')">다운로드</fluent-button>` : ''}
                </div>
                ${keywordsHtml}
            `;

            packageList.appendChild(card);
        }

        if (packageList.children.length === 0) {
            packageList.innerHTML = '<div class="loading">패키지가 없습니다.</div>';
        }
    } catch (e) {
        console.error('Failed to load packages:', e);
        packageList.innerHTML = '<div class="loading">패키지를 불러올 수 없습니다.</div>';
    }
}

// Add package to VCC
function addPackageToVCC(packageId) {
    window.location.href = `vcc://vpm/addRepo?url=${encodeURIComponent(listingUrl)}`;
}

// Initialize
loadPackages();
