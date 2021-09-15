﻿// fork https://github.com/jaywcjlove/nginx-editor

const tokenConf = {
    defaultToken: 'source',
    ignoreCase: true,
    brackets: [{ open: '{', close: '}', token: 'delimiter.bracket' }],

    tokenizer: {
        root: [
            [/(")/, 'delimiter.bracket'],
            // [/[{}()[\]]/, "@brackets"],
            [/[;,.]/, 'delimiter'],
            [/\\.* |~|~\*|!~|!~\*/, 'string.regexp'],
            [/\b\d+\w+\b/, 'number'],
            [/\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}(:\d{1,5})?\b/, 'number'],
            [/\b(ip_hash|upstream|server)\b/, 'http.upstream'],
            [/\b(add_header|expires|server_tokens|autoindex|autoindex_exact_size|autoindex_localtime)\b/, 'http.headers'],
            [/\b(map|map_hash_max_size|map_hash_bucket_size)\b/, 'module.http'],
            [/\b(http|stream)\b/, 'module.http'],
            [
                /\b(gzip|gzip_buffers|gzip_comp_level|gzip_disable|gzip_http.version|gzip_min_length|gzip_proxied|gzip_types|gzip_vary)\b/,
                'module.http',
            ],
            [/\s(on|off)\b/, 'module.main'],
            [/\b(access_log|log_format)\s/, 'module.log'],
            [
                /\b(proxy_buffer_size|proxy_buffering|proxy_buffers|proxy_busy_buffers_size|proxy_cache|proxy_cache_background_update|proxy_cache_bypass|proxy_cache_convert_head|proxy_cache_key|proxy_cache_lock|proxy_cache_lock_age|proxy_cache_lock_timeout|proxy_cache_max_range_offset|proxy_cache_methods|proxy_cache_min_uses|proxy_cache_path|proxy_cache_purge|proxy_cache_revalidate|proxy_cache_use_stale|proxy_cache_valid|proxy_connect_timeout|proxy_headers_hash_bucket_size|proxy_headers_hash_max_size|proxy_hide_header|proxy_http_version|proxy_ignore_client_abort|proxy_intercept_errors|proxy_max_temp_file_size|proxy_method|proxy_next_upstream|proxy_next_upstream_tries|proxy_next_upstream_timeout|proxy_pass|proxy_pass_header|proxy_pass_request_body|proxy_pass_request_headers|proxy_read_timeout|proxy_redirect|proxy_redirect_errors|proxy_send_lowat|proxy_send_timeout|proxy_set_body|proxy_set_header|proxy_store|proxy_store_access|proxy_temp_file_write_size|proxy_t|emp_pathproxy_upstream_fail_timeout|proxy_upstream_max_fails|proxy_temp_path|proxy_ssl_certificate|proxy_ssl_certificate_key|proxy_ssl_protocols|proxy_ssl_ciphers|proxy_ssl_verify|proxy_ssl_trusted_certificate|proxy_ssl_verify_depth|proxy_ssl_session_reuse|rewrite|return|break)\b/,
                'http.proxy',
            ],
            [
                /\b(ssl|ssl_buffer_size|ssl_certificate|ssl_certificate_key|ssl_ciphers|ssl_client_certificate|ssl_crl|ssl_dhparam|ssl_ecdh_curve|ssl_password_file|ssl_prefer_server_ciphers|ssl_protocols|ssl_session_cache|ssl_session_ticket_key|ssl_session_tickets|ssl_session_timeout|ssl_stapling|ssl_stapling_file|ssl_stapling_responder|ssl_stapling_verify|ssl_trusted_certificate|ssl_verify_client|ssl_verify_depth)\b/,
                'module.http',
            ],
            [
                /\b(daemon|env|debug_points|error_log|log_not_found|include|lock_file|master_process|pid|ssl_engine|timer_resolution|user|worker_cpu_affinity|worker_priority|worker_processes|worker_rlimit_core|worker_rlimit_nofile|worker_rlimit_sigpending|working_directory|try_files)\b/,
                'module.main',
            ],
            [
                /\b(index|alias|chunked_transfer_encoding|client_body_in_file_only|client_body_buffer_size|client_body_temp_path|client_body_timeout|client_header_buffer_size|client_header_timeout|types_hash_max_size|client_max_body_size|default_type|error_page|index |internal|keepalive_timeout|keepalive_requests|large_client_header_buffers|limit_except|limit_rate|listen|location|msie_padding|msie_refresh|optimize_server_names|port_in_redirect|recursive_error_pages|reset_timedout_connection|resolver|resolver_timeout|root|satisfy_any|send_timeout|sendfile|sendfile_max_chunk|server|server_name|server_names_hash_max_size|server_names_hash_bucket_size|tcp_nodelay|tcp_nopush|types |try_files|charset|stub_status|allow|deny)\s/,
                'module.http',
            ],
            [
                /\b(accept_mutex|accept_mutex_delay|debug_connection|devpoll_changes|devpoll_events|epoll_events|kqueue_changes|kqueue_events|multi_accept|rtsig_signo|rtsig_overflow_events|rtsig_overflow_test|rtsig_overflow_threshold|use|worker_connections)\b/,
                'module.events',
            ],
            [/\b(add_before_body|add_after_body|addition_types)\b/, 'module.http.addition'],
            [/\b(events)\b/, 'module.events'],
            [
                /\b(fastcgi_index|fastcgi_hide_header|fastcgi_ignore_client_abort|fastcgi_intercept_errors|fastcgi_param|fastcgi_pass|fastcgi_pass_header|fastcgi_read_timeout|fastcgi_redirect_errors|fa|stcgi_storefastcgi_store_access|fastcgi_buffers|fastcgi_buffers_size|fastcgi_temp_path|fastcgi_buffer_size|fastcgi_connect_timeout|fastcgi_send_timeout|fastcgi_split_path_info|fastcgi_busy_buffers_size|fastcgi_temp_file_write_size)\b/,
                'module.http',
            ],
            [/\b(if)\b/, 'module.condition'],
            [/\$\w+/, 'variable'],
            [/#.*$/, 'comment'],
            // { include: "@numbers" },
        ],
        comment: [[/#.*$/, 'comment']],
        // urldeclaration: [
        //     ['[^)\r\n]+', 'string'],
        //     ['\\)', { token: 'delimiter.parenthesis', next: '@pop' }]
        // ],
        numbers: [
            ['-?(\\d*\\.)?\\d+([eE][\\-+]?\\d+)?', { token: 'attribute.value.number', next: '@units' }],
            ['#[0-9a-fA-F_]+(?!\\w)', 'attribute.value.hex'],
        ],
        units: [['(M)?', 'attribute.value.unit', '@pop']],
    },
};

const themeConfig1 = {
    colors: {
        // 'attribute.value.unit': '#68217a'
    },
    base: 'vs-dark',
    inherit: true,
    rules: [
        {
            token: 'module.http',
            foreground: '#00bbbb',
        },
        {
            token: 'module.events',
            foreground: '#00bbbb',
        },
        {
            token: 'http.headers',
            foreground: '#00bbbb',
        },
        {
            token: 'http.proxy',
            foreground: '#58f18e',
        },
        {
            token: 'module.main',
            foreground: '#c152e4',
            fontStyle: 'bold',
        },
        {
            token: 'module.log',
            foreground: '#dd9970',
        },
        {
            token: 'module.condition',
            foreground: '#b038c4',
        },
        {
            token: 'module.http.addition',
            foreground: '#c152e4',
        },
        {
            token: 'keywords',
            foreground: '#9effff',
            fontStyle: 'bold',
        },
        {
            token: 'http.upstream',
            foreground: '#0078d0',
            fontStyle: 'bold',
        },
        {
            token: 'identifier',
            foreground: '#8e44ad',
        },
        {
            token: 'delimiter.bracket',
            foreground: '#d9d9d9',
        },
        {
            token: 'delimiter',
            foreground: '#d9d9d9',
        },
    ],
};

const themeConfig2 = {
    colors: {
        // 'attribute.value.unit': '#68217a'
    },
    base: 'vs',
    inherit: true,
    rules: [
        {
            token: 'variable',
            foreground:'#0bc5e5'
        },
        {
            token: 'module.http',
            foreground: '#b038c4',
        },
        {
            token: 'module.events',
            foreground: '#b038c4',
        },
        {
            token: 'http.headers',
            foreground: '#b038c4',
        },
        {
            token: 'http.proxy',
            foreground: '#df7b00',
        },
        {
            token: 'module.main',
            foreground: '#c152e4',
            fontStyle: 'bold',
        },
        {
            token: 'module.log',
            foreground: '#dd9970',
        },
        {
            token: 'module.condition',
            foreground: '#b038c4',
        },
        {
            token: 'module.http.addition',
            foreground: '#c152e4',
        },
        {
            token: 'keywords',
            foreground: '#9effff',
            fontStyle: 'bold',
        },
        {
            token: 'http.upstream',
            foreground: '#0078d0',
            fontStyle: 'bold',
        },
        {
            token: 'identifier',
            foreground: '#8e44ad',
        },
        {
            token: 'delimiter.bracket',
            foreground: '#555555',
        },
        {
            token: 'delimiter',
            foreground: '#555555',
        },
    ],
};

var editor,
    defaultLang = 'nginx',
    defaultContent = ss.lsStr("txt"),
    defaultTheme = ss.lsStr("vscode-theme") || 'vs';

require(['vs/editor/editor.main'], function () {

    monaco.languages.register({ id: defaultLang });
    monaco.languages.setLanguageConfiguration(defaultLang, {
        autoClosingPairs: [
            { open: '{', close: '}' },
            { open: '"', close: '"' },
        ],
    });
    monaco.languages.setMonarchTokensProvider(defaultLang, tokenConf);
    monaco.editor.defineTheme(themeConfig1.base, themeConfig1);
    monaco.editor.defineTheme(themeConfig2.base, themeConfig2);


    editor = monaco.editor.create(document.getElementById('editor'), ss.meConfig({
        value: defaultContent,
        language: defaultLang,
        theme: defaultTheme
    }));

    $('.nr-theme').change(function () {
        monaco.editor.setTheme(this.value);
        ss.ls["vscode-theme"] = this.value;
        ss.lsSave();
    }).val(defaultTheme);

    editor.onDidChangeModelContent(function (e) {
        clearTimeout(window.defer1);
        window.defer1 = setTimeout(function () {
            ss.ls["txt"] = editor.getValue();
            ss.lsSave();
        }, 1000 * 1)
    });

    editor.addCommand(monaco.KeyMod.Shift | monaco.KeyMod.Alt | monaco.KeyCode.KEY_F, function () {
        $('#btnFormatterNginxConf')[0].click();
    });

    $(window).on("load resize", function () {
        var ch = $(window).height() - $('#editor').offset().top - 20;
        $('#editor').css('height', Math.max(200, ch));
    });
});

//接收文件
ss.receiveFiles(function (files) {
    var file = files[0];
    var reader = new FileReader();
    reader.onload = function (e) {
        ss.keepSetValue(editor, e.target.result);
    };
    reader.readAsText(file);
});

$('#btnFormatterNginxConf').click(function () {
    var indent = "    ";
    modifyOptions({ INDENTATION: indent });
    var cleanLines = clean_lines(editor.getValue());
    modifyOptions({ trailingBlankLines: false });
    cleanLines = join_opening_bracket(cleanLines);
    cleanLines = perform_indentation(cleanLines, indent);

    ss.keepSetValue(editor, cleanLines.join("\n"));
});